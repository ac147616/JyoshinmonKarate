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
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "EmergencyContactName");
            ViewData["MembershipId"] = new SelectList(_context.Memberships, "MembershipId", "AgeGroup");
            return View();
        }

        // POST: MemberMemberships/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberMembershipId,MemberId,MembershipId,StartDate,EndDate,MembershipStatus")] MemberMembership memberMembership)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberMembership);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "EmergencyContactName", memberMembership.MemberId);
            ViewData["MembershipId"] = new SelectList(_context.Memberships, "MembershipId", "AgeGroup", memberMembership.MembershipId);
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
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "EmergencyContactName", memberMembership.MemberId);
            ViewData["MembershipId"] = new SelectList(_context.Memberships, "MembershipId", "AgeGroup", memberMembership.MembershipId);
            return View(memberMembership);
        }

        // POST: MemberMemberships/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberMembershipId,MemberId,MembershipId,StartDate,EndDate,MembershipStatus")] MemberMembership memberMembership)
        {
            if (id != memberMembership.MemberMembershipId)
            {
                return NotFound();
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
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "EmergencyContactName", memberMembership.MemberId);
            ViewData["MembershipId"] = new SelectList(_context.Memberships, "MembershipId", "AgeGroup", memberMembership.MembershipId);
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
    }
}
