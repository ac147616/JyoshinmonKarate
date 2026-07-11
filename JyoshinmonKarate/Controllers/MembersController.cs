using JyoshinmonKarate.Areas.Identity.Data;
using JyoshinmonKarate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;


namespace JyoshinmonKarate.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        private readonly JyoshinmonKarateContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MembersController(JyoshinmonKarateContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Members
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 20;

            var membersQuery = _context.Members
                .Include(m => m.Club)
                .Include(m => m.Belt)
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName);

            int totalMembers = await membersQuery.CountAsync();

            var members = await membersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalMembers / (double)pageSize);

            return View(members);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Member> allMembers = await _context.Members
                .Include("Belt")
                .Include("Club")
                .ToListAsync();

            Member member = null;

            foreach (Member item in allMembers)
            {
                if (item.MemberId == id.Value)
                {
                    member = item;
                    break;
                }
            }

            if (member == null)
            {
                return NotFound();
            }

            if (!CanEditMember(member))
            {
                return NotFound();
            }

            string membershipName = "Not assigned";

            List<MemberMembership> allMemberships = await _context.MemberMemberships
                .Include("Membership")
                .ToListAsync();

            DateTime latestMembershipStart = DateTime.MinValue;

            foreach (MemberMembership record in allMemberships)
            {
                if (record.MemberId == member.MemberId)
                {
                    if (record.StartDate > latestMembershipStart)
                    {
                        latestMembershipStart = record.StartDate;

                        if (record.Membership != null)
                        {
                            membershipName = record.Membership.MembershipName;
                        }
                    }
                }
            }

            string lastClassAttended = "No attendance yet";

            List<Attendance> allAttendances = await _context.Attendances.ToListAsync();

            DateTime latestAttendanceDate = DateTime.MinValue;

            foreach (Attendance attendance in allAttendances)
            {
                if (attendance.MemberId == member.MemberId)
                {
                    if (attendance.Date > latestAttendanceDate)
                    {
                        latestAttendanceDate = attendance.Date;
                        lastClassAttended = attendance.Date.ToString("dd MMM yyyy");
                    }
                }
            }

            decimal paymentDue = 0;

            List<Payment> allPayments = await _context.Payments.ToListAsync();

            foreach (Payment payment in allPayments)
            {
                if (payment.MemberId == member.MemberId)
                {
                    if (payment.Status.ToString() != "Paid")
                    {
                        paymentDue = paymentDue + payment.Amount;
                    }
                }
            }

            ViewBag.MembershipName = membershipName;
            ViewBag.LastClassAttended = lastClassAttended;
            ViewBag.PaymentDue = paymentDue;

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            Member member = new Member();

            member.DateJoined = DateTime.Today;
            member.Status = MemberStatus.Active;

            if (!User.IsInRole("Admin"))
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                member.UserId = currentUserId;
                member.BeltId = 1;
                member.DateJoined = DateTime.Today;
                member.Status = MemberStatus.Active;
                member.ProfilePhotoPath = null;
            }

            PopulateCreateViewData(member);

            return View(member);
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("MemberId,UserId,ClubId,BeltId,BeltSize,FirstName,LastName,DateOfBirth,Gender,Weight,Height,DateJoined,EmergencyContactName,EmergencyContactPhone,Status")]
    Member member,
    IFormFile? profilePhoto)
        {
            ModelState.Remove("ProfilePhotoPath");

            if (!User.IsInRole("Admin"))
            {
                ModelState.Remove("UserId");
                ModelState.Remove("BeltId");
                ModelState.Remove("DateJoined");
                ModelState.Remove("Status");

                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                member.UserId = currentUserId;
                member.BeltId = 1;
                member.DateJoined = DateTime.Today;
                member.Status = MemberStatus.Active;
                member.ProfilePhotoPath = null;
            }

            if (!IsDateBetween1900AndToday(member.DateOfBirth))
            {
                ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in the future.");
            }

            if (User.IsInRole("Admin"))
            {
                if (!IsDateBetween1900AndToday(member.DateJoined))
                {
                    ModelState.AddModelError("DateJoined", "Date joined cannot be in the future.");
                }
            }

            ModelState.Remove("Belt");
            ModelState.Remove("Club");
            ModelState.Remove("User");
            ModelState.Remove("Payments");
            ModelState.Remove("Attendances");
            ModelState.Remove("MemberGradings");
            ModelState.Remove("MemberMemberships");
            ModelState.Remove("ProfilePhotoPath");
            ModelState.Remove("ProfilePhotoFile");
            ModelState.Remove("profilePhoto");

            if (ModelState.IsValid)
            {
                if (profilePhoto != null && profilePhoto.Length > 0)
                {
                    member.ProfilePhotoPath = await SaveMemberPhoto(profilePhoto);
                }

                _context.Add(member);
                await _context.SaveChangesAsync();

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Details), new { id = member.MemberId });
            }

            PopulateCreateViewData(member);
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Belt)
                .Include(m => m.Club)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                return NotFound();
            }

            if (!CanEditMember(member))
            {
                return Forbid();
            }

            ViewData["BeltId"] = new SelectList(_context.Belts, "BeltId", "BeltName", member.BeltId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName", member.ClubId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", member.UserId);

            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    [Bind("MemberId,UserId,ClubId,BeltId,BeltSize,FirstName,LastName,DateOfBirth,Gender,Weight,Height,DateJoined,EmergencyContactName,EmergencyContactPhone,Status")]
    Member member,
    IFormFile? profilePhoto,
    bool removeProfilePhoto)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }

            var existingMember = await _context.Members.FindAsync(id);

            if (existingMember == null)
            {
                return NotFound();
            }

            if (!CanEditMember(existingMember))
            {
                return Forbid();
            }

            if (!IsDateBetween1900AndToday(member.DateOfBirth))
            {
                ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in the future.");
            }

            if (User.IsInRole("Admin"))
            {
                if (!IsDateBetween1900AndToday(member.DateJoined))
                {
                    ModelState.AddModelError("DateJoined", "Date joined cannot be in the future.");
                }
            }

            ModelState.Remove("ProfilePhotoPath");

            if (!User.IsInRole("Admin"))
            {
                ModelState.Remove("UserId");
                ModelState.Remove("BeltId");
                ModelState.Remove("DateJoined");
                ModelState.Remove("Status");
            }

            ModelState.Remove("Belt");
            ModelState.Remove("Club");
            ModelState.Remove("User");
            ModelState.Remove("Payments");
            ModelState.Remove("Attendances");
            ModelState.Remove("MemberGradings");
            ModelState.Remove("MemberMemberships");
            ModelState.Remove("ProfilePhotoPath");
            ModelState.Remove("ProfilePhotoFile");
            ModelState.Remove("profilePhoto");

            if (ModelState.IsValid)
            {
                try
                {
                    if (User.IsInRole("Admin"))
                    {
                        existingMember.UserId = member.UserId;
                        existingMember.ClubId = member.ClubId;
                        existingMember.BeltId = member.BeltId;  
                        existingMember.Status = member.Status;
                        existingMember.DateJoined = member.DateJoined;
                    }

                    existingMember.FirstName = member.FirstName;
                    existingMember.LastName = member.LastName;
                    existingMember.DateOfBirth = member.DateOfBirth;
                    existingMember.Gender = member.Gender;
                    existingMember.Weight = member.Weight;
                    existingMember.Height = member.Height;
                    existingMember.BeltSize = member.BeltSize;
                    existingMember.EmergencyContactName = member.EmergencyContactName;
                    existingMember.EmergencyContactPhone = member.EmergencyContactPhone;

                    if (removeProfilePhoto)
                    {
                        DeleteMemberPhotoFile(existingMember.ProfilePhotoPath);
                        existingMember.ProfilePhotoPath = null;
                    }

                    if (profilePhoto != null && profilePhoto.Length > 0)
                    {
                        DeleteMemberPhotoFile(existingMember.ProfilePhotoPath);
                        existingMember.ProfilePhotoPath = await SaveMemberPhoto(profilePhoto);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Details), new { id = existingMember.MemberId });
            }

            ViewData["BeltId"] = new SelectList(_context.Belts, "BeltId", "BeltName", member.BeltId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName", member.ClubId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", member.UserId);

            return View(member);
        }

        // GET: Members/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Belt)
                .Include(m => m.Club)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CanEditMember(Member member)
        {
            if (User.IsInRole("Admin"))
            {
                return true;
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return member.UserId == currentUserId;
        }

        private bool IsDateBetween1900AndToday(DateTime date)
        {
            DateTime minimumDate = new DateTime(1900, 1, 1);
            DateTime maximumDate = DateTime.Today;

            return date >= minimumDate && date <= maximumDate;
        }
        private void PopulateCreateViewData(Member member)
        {
            ViewData["BeltId"] = new SelectList(_context.Belts, "BeltId", "BeltName", member.BeltId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName", member.ClubId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", member.UserId);

            ViewBag.CurrentUserDisplay = User.Identity?.Name;
            ViewBag.DefaultBeltName = "White";
        }
        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }

        private async Task<string> SaveMemberPhoto(IFormFile profilePhoto)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "member-photos");
            Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePhoto.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePhoto.CopyToAsync(stream);
            }

            return "/images/member-photos/" + fileName;
        }

        private void DeleteMemberPhotoFile(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath))
            {
                return;
            }

            string relativePath = photoPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
