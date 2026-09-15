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
    public class MemberGradingsController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public MemberGradingsController(JyoshinmonKarateContext context)
        {
            _context = context;
        }


        // GET: MemberGradings
        public async Task<IActionResult> Index(
            int memberId = 0,
            int gradingId = 0,
            int beltAfterId = 0,
            string result = "",
            int page = 1)
        {
            int pageSize = 20;

            var memberGradingsQuery = _context.MemberGradings
                .Include(m => m.BeltAfter)
                .Include(m => m.BeltBefore)
                .Include(m => m.Grading)
                    .ThenInclude(g => g.Club)
                .Include(m => m.Member)
                .AsQueryable();

            var membersQuery = _context.Members.AsQueryable();


            // If the user is not an Admin,
            // only show their own grading history.
            if (!User.IsInRole("Admin"))
            {
                string currentUserName = User.Identity.Name;

                var currentUser = await _context.Users
                    .FirstOrDefaultAsync(
                        u => u.UserName == currentUserName);

                if (currentUser == null)
                {
                    return NotFound();
                }

                memberGradingsQuery = memberGradingsQuery
                    .Where(
                        m => m.Member.UserId == currentUser.Id);

                membersQuery = membersQuery
                    .Where(
                        m => m.UserId == currentUser.Id);
            }


            // Filter by member.
            if (memberId != 0)
            {
                memberGradingsQuery = memberGradingsQuery
                    .Where(
                        m => m.MemberId == memberId);
            }


            // Filter by grading session.
            if (gradingId != 0)
            {
                memberGradingsQuery = memberGradingsQuery
                    .Where(
                        m => m.GradingId == gradingId);
            }


            // Filter by belt after grading.
            if (beltAfterId != 0)
            {
                memberGradingsQuery = memberGradingsQuery
                    .Where(
                        m => m.BeltAfterId == beltAfterId);
            }


            // Filter by grading status.
            if (result == "Pending")
            {
                memberGradingsQuery = memberGradingsQuery
                    .Where(
                        m => m.Status ==
                        GradingStatus.Pending);
            }
            else if (result == "Passed")
            {
                memberGradingsQuery = memberGradingsQuery
                    .Where(
                        m => m.Status ==
                        GradingStatus.Passed);
            }
            else if (result == "NotPassed")
            {
                memberGradingsQuery = memberGradingsQuery
                    .Where(
                        m => m.Status ==
                        GradingStatus.NotPassed);
            }


            memberGradingsQuery = memberGradingsQuery
                .OrderByDescending(
                    m => m.Grading.GradingDate)
                .ThenBy(
                    m => m.Member.FirstName)
                .ThenBy(
                    m => m.Member.LastName);


            int totalRecords =
                await memberGradingsQuery.CountAsync();


            var memberGradings = await memberGradingsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            // Member dropdown.
            var members = await membersQuery
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();

            List<SelectListItem> memberOptions =
                new List<SelectListItem>();

            foreach (Member member in members)
            {
                memberOptions.Add(
                    new SelectListItem
                    {
                        Value =
                            member.MemberId.ToString(),

                        Text =
                            member.FirstName
                            + " "
                            + member.LastName
                    });
            }


            // Grading session dropdown.
            var gradings = await _context.Gradings
                .Include(g => g.Club)
                .OrderByDescending(g => g.GradingDate)
                .ToListAsync();

            List<SelectListItem> gradingOptions =
                new List<SelectListItem>();

            foreach (Grading grading in gradings)
            {
                gradingOptions.Add(
                    new SelectListItem
                    {
                        Value =
                            grading.GradingId.ToString(),

                        Text =
                            grading.SessionName
                            + " | "
                            + grading.GradingDate
                                .ToString("dd MMM yyyy")
                            + " | "
                            + grading.Club.ClubName
                    });
            }


            // Belt dropdown.
            var belts = await _context.Belts
                .OrderBy(b => b.BeltName)
                .ToListAsync();

            List<SelectListItem> beltOptions =
                new List<SelectListItem>();

            foreach (Belt belt in belts)
            {
                beltOptions.Add(
                    new SelectListItem
                    {
                        Value =
                            belt.BeltId.ToString(),

                        Text =
                            belt.BeltName
                    });
            }


            ViewData["MemberId"] =
                new SelectList(
                    memberOptions,
                    "Value",
                    "Text",
                    memberId);

            ViewData["GradingId"] =
                new SelectList(
                    gradingOptions,
                    "Value",
                    "Text",
                    gradingId);

            ViewData["BeltAfterId"] =
                new SelectList(
                    beltOptions,
                    "Value",
                    "Text",
                    beltAfterId);


            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    totalRecords / (double)pageSize);

            ViewBag.SelectedMemberId = memberId;

            ViewBag.SelectedGradingId = gradingId;

            ViewBag.SelectedBeltAfterId = beltAfterId;

            ViewBag.SelectedResult = result;


            return View(memberGradings);
        }



        // GET: MemberGradings/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var memberGrading =
                await _context.MemberGradings
                    .Include(m => m.Member)
                    .Include(m => m.Grading)
                        .ThenInclude(g => g.Club)
                    .Include(m => m.BeltBefore)
                    .Include(m => m.BeltAfter)
                    .FirstOrDefaultAsync(
                        m => m.MemberGradingId == id);


            if (memberGrading == null)
            {
                return NotFound();
            }


            return View(memberGrading);
        }



        // GET: MemberGradings/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            MemberGrading memberGrading =
                new MemberGrading
                {
                    Status =
                        GradingStatus.Pending,

                    BeltAfterId = null
                };


            await PopulateFormLists(memberGrading);


            return View(memberGrading);
        }



        // POST: MemberGradings/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "MemberGradingId," +
                "GradingId," +
                "MemberId," +
                "BeltBeforeId")]
            MemberGrading memberGrading)
        {
            ModelState.Remove("Grading");
            ModelState.Remove("Member");
            ModelState.Remove("BeltBefore");
            ModelState.Remove("BeltAfter");


            /*
             * Creating a MemberGrading now means
             * selecting a member for an upcoming
             * grading session.
             *
             * They have not completed the grading yet.
             */
            memberGrading.Status =
                GradingStatus.Pending;

            memberGrading.BeltAfterId = null;


            // Check that the same member has not already
            // been selected for this grading session.
            bool gradingExists =
                await _context.MemberGradings
                    .AnyAsync(m =>
                        m.MemberId ==
                            memberGrading.MemberId
                        &&
                        m.GradingId ==
                            memberGrading.GradingId);


            if (gradingExists)
            {
                ModelState.AddModelError(
                    "",
                    "This member has already been selected for this grading.");
            }


            if (ModelState.IsValid)
            {
                _context.Add(memberGrading);

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index));
            }


            await PopulateFormLists(memberGrading);


            return View(memberGrading);
        }



        // GET: MemberGradings/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var memberGrading =
                await _context.MemberGradings
                    .FindAsync(id);


            if (memberGrading == null)
            {
                return NotFound();
            }


            await PopulateFormLists(memberGrading);


            return View(memberGrading);
        }



        // POST: MemberGradings/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "MemberGradingId," +
                "GradingId," +
                "MemberId," +
                "BeltBeforeId," +
                "BeltAfterId," +
                "Status")]
            MemberGrading memberGrading)
        {
            if (id != memberGrading.MemberGradingId)
            {
                return NotFound();
            }


            ModelState.Remove("Grading");
            ModelState.Remove("Member");
            ModelState.Remove("BeltBefore");
            ModelState.Remove("BeltAfter");


            /*
             * If the member has not yet graded,
             * there should be no Belt After.
             */
            if (memberGrading.Status ==
                GradingStatus.Pending)
            {
                memberGrading.BeltAfterId = null;
            }


            /*
             * If the member passed,
             * a new belt must be selected.
             */
            if (memberGrading.Status ==
                GradingStatus.Passed)
            {
                if (memberGrading.BeltAfterId == null)
                {
                    ModelState.AddModelError(
                        "BeltAfterId",
                        "Please select the new belt for a member who passed.");
                }
                else if (
                    memberGrading.BeltBeforeId ==
                    memberGrading.BeltAfterId)
                {
                    ModelState.AddModelError(
                        "BeltAfterId",
                        "The new belt must be different from the previous belt when the member has passed.");
                }
            }


            /*
             * If the member did not pass,
             * their belt remains unchanged.
             */
            if (memberGrading.Status ==
                GradingStatus.NotPassed)
            {
                memberGrading.BeltAfterId =
                    memberGrading.BeltBeforeId;
            }


            // Prevent duplicate MemberGrading records
            // for the same member and grading session.
            bool gradingExists =
                await _context.MemberGradings
                    .AnyAsync(m =>
                        m.MemberGradingId !=
                            memberGrading.MemberGradingId
                        &&
                        m.MemberId ==
                            memberGrading.MemberId
                        &&
                        m.GradingId ==
                            memberGrading.GradingId);


            if (gradingExists)
            {
                ModelState.AddModelError(
                    "",
                    "This member has already been selected for this grading.");
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberGrading);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberGradingExists(
                        memberGrading.MemberGradingId))
                    {
                        return NotFound();
                    }

                    throw;
                }


                return RedirectToAction(
                    nameof(Index));
            }


            await PopulateFormLists(memberGrading);


            return View(memberGrading);
        }



        // GET: MemberGradings/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var memberGrading =
                await _context.MemberGradings
                    .Include(m => m.Member)
                    .Include(m => m.Grading)
                        .ThenInclude(g => g.Club)
                    .Include(m => m.BeltBefore)
                    .Include(m => m.BeltAfter)
                    .FirstOrDefaultAsync(
                        m => m.MemberGradingId == id);


            if (memberGrading == null)
            {
                return NotFound();
            }


            return View(memberGrading);
        }



        // POST: MemberGradings/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var memberGrading =
                await _context.MemberGradings
                    .FindAsync(id);


            if (memberGrading != null)
            {
                _context.MemberGradings
                    .Remove(memberGrading);
            }


            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }



        /*
         * Populates the dropdown lists used
         * by the Create and Edit views.
         */
        private async Task PopulateFormLists(
            MemberGrading memberGrading)
        {
            // Members.
            var members = await _context.Members
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();


            List<SelectListItem> memberOptions =
                new List<SelectListItem>();


            foreach (Member member in members)
            {
                memberOptions.Add(
                    new SelectListItem
                    {
                        Value =
                            member.MemberId.ToString(),

                        Text =
                            member.FirstName
                            + " "
                            + member.LastName
                    });
            }


            // Grading sessions.
            var gradings = await _context.Gradings
                .Include(g => g.Club)
                .OrderByDescending(g => g.GradingDate)
                .ToListAsync();


            List<SelectListItem> gradingOptions =
                new List<SelectListItem>();


            foreach (Grading grading in gradings)
            {
                gradingOptions.Add(
                    new SelectListItem
                    {
                        Value =
                            grading.GradingId.ToString(),

                        Text =
                            grading.SessionName
                            + " | "
                            + grading.GradingDate
                                .ToString("dd MMM yyyy")
                            + " | "
                            + grading.Club.ClubName
                    });
            }


            // Belts.
            var belts = await _context.Belts
                .OrderBy(b => b.BeltName)
                .ToListAsync();


            List<SelectListItem> beltOptions =
                new List<SelectListItem>();


            foreach (Belt belt in belts)
            {
                beltOptions.Add(
                    new SelectListItem
                    {
                        Value =
                            belt.BeltId.ToString(),

                        Text =
                            belt.BeltName
                    });
            }


            ViewData["MemberId"] =
                new SelectList(
                    memberOptions,
                    "Value",
                    "Text",
                    memberGrading.MemberId);


            ViewData["GradingId"] =
                new SelectList(
                    gradingOptions,
                    "Value",
                    "Text",
                    memberGrading.GradingId);


            ViewData["BeltBeforeId"] =
                new SelectList(
                    beltOptions,
                    "Value",
                    "Text",
                    memberGrading.BeltBeforeId);


            ViewData["BeltAfterId"] =
                new SelectList(
                    beltOptions,
                    "Value",
                    "Text",
                    memberGrading.BeltAfterId);
        }



        private bool MemberGradingExists(int id)
        {
            return _context.MemberGradings
                .Any(
                    e => e.MemberGradingId == id);
        }
    }
}