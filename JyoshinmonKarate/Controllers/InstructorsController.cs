using JyoshinmonKarate.Areas.Identity.Data;
using JyoshinmonKarate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JyoshinmonKarate.Controllers
{
    [Authorize]
    public class InstructorsController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public InstructorsController(JyoshinmonKarateContext context)
        {
            _context = context;
        }

        // GET: Instructors
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 20;

            var instructorsQuery = _context.Instructors
                .Include(i => i.User)
                .Include(i => i.Club)
                .Include(i => i.Belt)
                .OrderBy(i => i.User.FirstName)
                .ThenBy(i => i.User.LastName);

            int totalInstructors = await instructorsQuery.CountAsync();

            var instructors = await instructorsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalInstructors / (double)pageSize);

            return View(instructors);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .Include(i => i.Club)
                .Include(i => i.Belt)
                .FirstOrDefaultAsync(i => i.InstructorId == id);

            if (instructor == null)
            {
                return NotFound();
            }

            if (!CanEditInstructor(instructor))
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            Instructor instructor = new Instructor();
            instructor.DateJoined = DateTime.Today;

            if (!User.IsInRole("Admin"))
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                instructor.UserId = currentUserId;
                instructor.BeltId = 1;
                instructor.DateJoined = DateTime.Today;
            }

            PopulateInstructorViewData(instructor);

            return View(instructor);
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructorId,UserId,ClubId,BeltId,DateJoined")] Instructor instructor)
        {
            if (!User.IsInRole("Admin"))
            {
                ModelState.Remove("UserId");
                ModelState.Remove("BeltId");
                ModelState.Remove("DateJoined");

                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                instructor.UserId = currentUserId;
                instructor.BeltId = 1;
                instructor.DateJoined = DateTime.Today;
            }

            if (User.IsInRole("Admin"))
            {
                if (!IsDateBetween1900AndToday(instructor.DateJoined))
                {
                    ModelState.AddModelError("DateJoined", "Date joined cannot be in the future.");
                }
            }

            ModelState.Remove("User");
            ModelState.Remove("Club");
            ModelState.Remove("Belt");
            ModelState.Remove("Schedules");

            bool instructorAlreadyExists = await _context.Instructors.AnyAsync(i => i.UserId == instructor.UserId);

            if (instructorAlreadyExists)
            {
                ModelState.AddModelError("UserId", "This user already has an instructor record.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateInstructorViewData(instructor);

            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .Include(i => i.Club)
                .Include(i => i.Belt)
                .FirstOrDefaultAsync(i => i.InstructorId == id);

            if (instructor == null)
            {
                return NotFound();
            }

            if (!CanEditInstructor(instructor))
            {
                return Forbid();
            }

            PopulateInstructorViewData(instructor);

            return View(instructor);
        }

        // POST: Instructors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstructorId,UserId,ClubId,BeltId,DateJoined")] Instructor instructor)
        {
            if (id != instructor.InstructorId)
            {
                return NotFound();
            }

            var existingInstructor = await _context.Instructors.FindAsync(id);

            if (existingInstructor == null)
            {
                return NotFound();
            }

            if (!CanEditInstructor(existingInstructor))
            {
                return Forbid();
            }

            if (!User.IsInRole("Admin"))
            {
                ModelState.Remove("UserId");
                ModelState.Remove("BeltId");
                ModelState.Remove("DateJoined");
            }

            if (User.IsInRole("Admin"))
            {
                if (!IsDateBetween1900AndToday(instructor.DateJoined))
                {
                    ModelState.AddModelError("DateJoined", "Date joined cannot be in the future.");
                }

                bool instructorAlreadyExists = await _context.Instructors.AnyAsync(i => i.UserId == instructor.UserId && i.InstructorId != instructor.InstructorId);

                if (instructorAlreadyExists)
                {
                    ModelState.AddModelError("UserId", "This user already has an instructor record.");
                }
            }

            ModelState.Remove("User");
            ModelState.Remove("Club");
            ModelState.Remove("Belt");
            ModelState.Remove("Schedules");

            if (ModelState.IsValid)
            {
                try
                {
                    if (User.IsInRole("Admin"))
                    {
                        existingInstructor.UserId = instructor.UserId;
                        existingInstructor.ClubId = instructor.ClubId;
                        existingInstructor.BeltId = instructor.BeltId;
                        existingInstructor.DateJoined = instructor.DateJoined;
                    }
                    else
                    {
                        existingInstructor.ClubId = instructor.ClubId;
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.InstructorId))
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

            PopulateInstructorViewData(instructor);

            return View(instructor);
        }

        // GET: Attendances/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Member)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Club)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);

            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // POST: Instructors/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool CanEditInstructor(Instructor instructor)
        {
            if (User.IsInRole("Admin"))
            {
                return true;
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return instructor.UserId == currentUserId;
        }

        private bool IsDateBetween1900AndToday(DateTime date)
        {
            DateTime minimumDate = new DateTime(1900, 1, 1);
            DateTime maximumDate = DateTime.Today;

            return date >= minimumDate && date <= maximumDate;
        }

        private void PopulateInstructorViewData(Instructor instructor)
        {
            ViewData["BeltId"] = new SelectList(_context.Belts, "BeltId", "BeltName", instructor.BeltId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName", instructor.ClubId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", instructor.UserId);

            ViewBag.CurrentUserDisplay = User.Identity?.Name;
            ViewBag.DefaultBeltName = "White";
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(i => i.InstructorId == id);
        }
    }
}