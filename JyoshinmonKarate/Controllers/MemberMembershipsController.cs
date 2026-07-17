using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JyoshinmonKarate.Areas.Identity.Data;
using JyoshinmonKarate.Models;
using Microsoft.AspNetCore.Authorization;

namespace JyoshinmonKarate.Controllers
{
    [Authorize]
    public class MemberMembershipsController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public MemberMembershipsController(JyoshinmonKarateContext context)
        {
            _context = context;
        }

        // GET: MemberMemberships
        public async Task<IActionResult> Index(int memberId = 0, int membershipId = 0, string membershipStatus = "", string startDate = "", string endDate = "", int page = 1)
        {
            int pageSize = 20;

            var memberMembershipsQuery = _context.MemberMemberships
                .Include(m => m.Member)
                .Include(m => m.Membership)
                .AsQueryable();

            var membersQuery = _context.Members.AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                string currentUserName = User.Identity.Name;

                var currentUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == currentUserName);

                if (currentUser == null)
                {
                    return NotFound();
                }

                memberMembershipsQuery = memberMembershipsQuery
                    .Where(m => m.Member.UserId == currentUser.Id);

                membersQuery = membersQuery
                    .Where(m => m.UserId == currentUser.Id);
            }

            if (memberId != 0)
            {
                memberMembershipsQuery = memberMembershipsQuery
                    .Where(m => m.MemberId == memberId);
            }

            if (membershipId != 0)
            {
                memberMembershipsQuery = memberMembershipsQuery
                    .Where(m => m.MembershipId == membershipId);
            }

            if (!string.IsNullOrEmpty(membershipStatus))
            {
                MembershipStatus selectedStatus;

                if (Enum.TryParse(membershipStatus, out selectedStatus))
                {
                    memberMembershipsQuery = memberMembershipsQuery
                        .Where(m => m.MembershipStatus == selectedStatus);
                }
            }

            DateTime selectedStartDate;

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out selectedStartDate))
            {
                memberMembershipsQuery = memberMembershipsQuery
                    .Where(m => m.StartDate >= selectedStartDate.Date);
            }

            DateTime selectedEndDate;

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out selectedEndDate))
            {
                memberMembershipsQuery = memberMembershipsQuery
                    .Where(m => m.EndDate <= selectedEndDate.Date);
            }

            memberMembershipsQuery = memberMembershipsQuery
                .OrderByDescending(m => m.StartDate)
                .ThenBy(m => m.Member.FirstName)
                .ThenBy(m => m.Member.LastName);

            int totalRecords = await memberMembershipsQuery.CountAsync();

            var memberMemberships = await memberMembershipsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var members = await membersQuery
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();

            List<SelectListItem> memberOptions = new List<SelectListItem>();

            foreach (Member member in members)
            {
                memberOptions.Add(new SelectListItem
                {
                    Value = member.MemberId.ToString(),
                    Text = member.FirstName + " " + member.LastName
                });
            }

            var memberships = await _context.Memberships
                .OrderBy(m => m.MembershipName)
                .ToListAsync();

            List<SelectListItem> membershipOptions = new List<SelectListItem>();

            foreach (Membership membership in memberships)
            {
                membershipOptions.Add(new SelectListItem
                {
                    Value = membership.MembershipId.ToString(),
                    Text = membership.MembershipName
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", memberId);
            ViewData["MembershipId"] = new SelectList(membershipOptions, "Value", "Text", membershipId);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.SelectedMemberId = memberId;
            ViewBag.SelectedMembershipId = membershipId;
            ViewBag.SelectedMembershipStatus = membershipStatus;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(memberMemberships);
        }

        // GET: MemberMemberships/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberMembership = await _context.MemberMemberships
                .Include(m => m.Member)
                .Include(m => m.Membership)
                .FirstOrDefaultAsync(m => m.MemberMembershipId == id);

            if (memberMembership == null)
            {
                return NotFound();
            }

            return View(memberMembership);
        }

        // GET: MemberMemberships/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            MemberMembership memberMembership = new MemberMembership();
            memberMembership.StartDate = DateTime.Today;
            memberMembership.EndDate = DateTime.Today;
            memberMembership.MembershipStatus = MembershipStatus.Active;

            var members = await _context.Members
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();

            List<SelectListItem> memberOptions = new List<SelectListItem>();

            foreach (Member member in members)
            {
                memberOptions.Add(new SelectListItem
                {
                    Value = member.MemberId.ToString(),
                    Text = member.FirstName + " " + member.LastName
                });
            }

            var memberships = await _context.Memberships
                .OrderBy(m => m.MembershipName)
                .ToListAsync();

            List<SelectListItem> membershipOptions = new List<SelectListItem>();

            foreach (Membership membership in memberships)
            {
                membershipOptions.Add(new SelectListItem
                {
                    Value = membership.MembershipId.ToString(),
                    Text = membership.MembershipName
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text");
            ViewData["MembershipId"] = new SelectList(membershipOptions, "Value", "Text");

            return View(memberMembership);
        }

        // POST: MemberMemberships/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberMembershipId,MemberId,MembershipId,StartDate,EndDate,MembershipStatus")] MemberMembership memberMembership)
        {
            ModelState.Remove("Member");
            ModelState.Remove("Membership");

            if (!IsDateBetween1900And2200(memberMembership.StartDate))
            {
                ModelState.AddModelError("StartDate", "Start date must be between 1900 and 2200.");
            }

            if (!IsDateBetween1900And2200(memberMembership.EndDate))
            {
                ModelState.AddModelError("EndDate", "End date must be between 1900 and 2200.");
            }

            if (memberMembership.EndDate < memberMembership.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date cannot be earlier than start date.");
            }

            bool membershipExists = await _context.MemberMemberships.AnyAsync(m =>
                m.MemberId == memberMembership.MemberId &&
                m.MembershipId == memberMembership.MembershipId &&
                memberMembership.StartDate <= m.EndDate &&
                memberMembership.EndDate >= m.StartDate);

            if (membershipExists)
            {
                ModelState.AddModelError("", "This member already has this membership during the selected dates.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(memberMembership);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var members = await _context.Members
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();

            List<SelectListItem> memberOptions = new List<SelectListItem>();

            foreach (Member member in members)
            {
                memberOptions.Add(new SelectListItem
                {
                    Value = member.MemberId.ToString(),
                    Text = member.FirstName + " " + member.LastName
                });
            }

            var memberships = await _context.Memberships
                .OrderBy(m => m.MembershipName)
                .ToListAsync();

            List<SelectListItem> membershipOptions = new List<SelectListItem>();

            foreach (Membership membership in memberships)
            {
                membershipOptions.Add(new SelectListItem
                {
                    Value = membership.MembershipId.ToString(),
                    Text = membership.MembershipName
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", memberMembership.MemberId);
            ViewData["MembershipId"] = new SelectList(membershipOptions, "Value", "Text", memberMembership.MembershipId);

            return View(memberMembership);
        }

        // GET: MemberMemberships/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberMembership = await _context.MemberMemberships.FindAsync(id);

            if (memberMembership == null)
            {
                return NotFound();
            }

            var members = await _context.Members
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();

            List<SelectListItem> memberOptions = new List<SelectListItem>();

            foreach (Member member in members)
            {
                memberOptions.Add(new SelectListItem
                {
                    Value = member.MemberId.ToString(),
                    Text = member.FirstName + " " + member.LastName
                });
            }

            var memberships = await _context.Memberships
                .OrderBy(m => m.MembershipName)
                .ToListAsync();

            List<SelectListItem> membershipOptions = new List<SelectListItem>();

            foreach (Membership membership in memberships)
            {
                membershipOptions.Add(new SelectListItem
                {
                    Value = membership.MembershipId.ToString(),
                    Text = membership.MembershipName
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", memberMembership.MemberId);
            ViewData["MembershipId"] = new SelectList(membershipOptions, "Value", "Text", memberMembership.MembershipId);

            return View(memberMembership);
        }

        // POST: MemberMemberships/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberMembershipId,MemberId,MembershipId,StartDate,EndDate,MembershipStatus")] MemberMembership memberMembership)
        {
            if (id != memberMembership.MemberMembershipId)
            {
                return NotFound();
            }

            ModelState.Remove("Member");
            ModelState.Remove("Membership");

            if (!IsDateBetween1900And2200(memberMembership.StartDate))
            {
                ModelState.AddModelError("StartDate", "Start date must be between 1900 and 2200.");
            }

            if (!IsDateBetween1900And2200(memberMembership.EndDate))
            {
                ModelState.AddModelError("EndDate", "End date must be between 1900 and 2200.");
            }

            if (memberMembership.EndDate < memberMembership.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date cannot be earlier than start date.");
            }

            bool membershipExists = await _context.MemberMemberships.AnyAsync(m =>
                m.MemberMembershipId != memberMembership.MemberMembershipId &&
                m.MemberId == memberMembership.MemberId &&
                m.MembershipId == memberMembership.MembershipId &&
                memberMembership.StartDate <= m.EndDate &&
                memberMembership.EndDate >= m.StartDate);

            if (membershipExists)
            {
                ModelState.AddModelError("", "This member already has this membership during the selected dates.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberMembership);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberMembershipExists(memberMembership.MemberMembershipId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            var members = await _context.Members
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();

            List<SelectListItem> memberOptions = new List<SelectListItem>();

            foreach (Member member in members)
            {
                memberOptions.Add(new SelectListItem
                {
                    Value = member.MemberId.ToString(),
                    Text = member.FirstName + " " + member.LastName
                });
            }

            var memberships = await _context.Memberships
                .OrderBy(m => m.MembershipName)
                .ToListAsync();

            List<SelectListItem> membershipOptions = new List<SelectListItem>();

            foreach (Membership membership in memberships)
            {
                membershipOptions.Add(new SelectListItem
                {
                    Value = membership.MembershipId.ToString(),
                    Text = membership.MembershipName
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", memberMembership.MemberId);
            ViewData["MembershipId"] = new SelectList(membershipOptions, "Value", "Text", memberMembership.MembershipId);

            return View(memberMembership);
        }

        // GET: MemberMemberships/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberMembership = await _context.MemberMemberships
                .Include(m => m.Member)
                .Include(m => m.Membership)
                .FirstOrDefaultAsync(m => m.MemberMembershipId == id);

            if (memberMembership == null)
            {
                return NotFound();
            }

            return View(memberMembership);
        }

        // POST: MemberMemberships/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberMembership = await _context.MemberMemberships.FindAsync(id);
            if (memberMembership != null)
            {
                _context.MemberMemberships.Remove(memberMembership);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberMembershipExists(int id)
        {
            return _context.MemberMemberships.Any(e => e.MemberMembershipId == id);
        }

        private bool IsDateBetween1900And2200(DateTime date)
        {
            DateTime minimumDate = new DateTime(1900, 1, 1);
            DateTime maximumDate = new DateTime(2200, 1, 1);

            return date.Date >= minimumDate && date.Date <= maximumDate;
        }
    }
}
