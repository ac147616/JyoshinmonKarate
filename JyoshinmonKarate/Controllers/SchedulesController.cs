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

namespace JyoshinmonKarate.Controllers
{
    [Authorize]
    public class SchedulesController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public SchedulesController(JyoshinmonKarateContext context)
        {
            _context = context;
        }

        // GET: Schedules
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var schedules = await _context.Schedules
                .Include(s => s.Club)
                .Include(s => s.Instructor)
                .ThenInclude(i => i.User)
                .OrderBy(s => s.Club.ClubName)
                .ThenBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return View(schedules);
        }

        // GET: Schedules/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Club)
                .Include(s => s.Instructor)
                .ThenInclude(i => i.User)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: Schedules/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName");

            var instructors = _context.Instructors
                .Include(i => i.User)
                .Select(i => new
                {
                    i.InstructorId,
                    FullName = i.User.FirstName + " " + i.User.LastName
                })
                .OrderBy(i => i.FullName)
                .ToList();

            ViewData["InstructorId"] = new SelectList(instructors, "InstructorId", "FullName");

            return View();
        }

        // POST: Schedules/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleId,ClubId,InstructorId,Level,DayOfWeek,StartTime,EndTime")] Schedule schedule)
        {
            ModelState.Remove("Club");
            ModelState.Remove("Instructor");
            ModelState.Remove("Attendances");

            if (schedule.EndTime.TimeOfDay <= schedule.StartTime.TimeOfDay)
            {
                ModelState.AddModelError("EndTime", "End time must be later than the start time.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubName", schedule.ClubId);

            var instructors = await _context.Instructors
                .Include(i => i.User)
                .Select(i => new
                {
                    i.InstructorId,
                    FullName = i.User.FirstName + " " + i.User.LastName
                })
                .OrderBy(i => i.FullName)
                .ToListAsync();

            ViewData["InstructorId"] = new SelectList(instructors, "InstructorId", "FullName", schedule.InstructorId);

            return View(schedule);
        }

        // GET: Schedules/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "Address", schedule.ClubId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "UserId", schedule.InstructorId);
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,ClubId,InstructorId,Level,DayOfWeek,StartTime,EndTime")] Schedule schedule)
        {
            if (id != schedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ScheduleId))
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
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "Address", schedule.ClubId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "UserId", schedule.InstructorId);
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Club)
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ScheduleId == id);
        }
    }
}
