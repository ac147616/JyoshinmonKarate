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
    public class AttendancesController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public AttendancesController(JyoshinmonKarateContext context)
        {
            _context = context;
        }

        // GET: Attendances
        public async Task<IActionResult> Index(int memberId = 0, int scheduleId = 0, string startDate = "", string endDate = "", int page = 1)
        {
            int pageSize = 20;

            var attendancesQuery = _context.Attendances
                .Include(a => a.Member)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Club)
                .AsQueryable();

            var membersQuery = _context.Members.AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                string currentUserName = User.Identity.Name;

                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUserName);

                if (currentUser == null)
                {
                    return NotFound();
                }

                attendancesQuery = attendancesQuery.Where(a => a.Member.UserId == currentUser.Id);
                membersQuery = membersQuery.Where(m => m.UserId == currentUser.Id);
            }

            if (memberId != 0)
            {
                attendancesQuery = attendancesQuery.Where(a => a.MemberId == memberId);
            }

            if (scheduleId != 0)
            {
                attendancesQuery = attendancesQuery.Where(a => a.ScheduleId == scheduleId);
            }

            DateTime selectedStartDate;

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out selectedStartDate))
            {
                attendancesQuery = attendancesQuery.Where(a => a.Date >= selectedStartDate.Date);
            }

            DateTime selectedEndDate;

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out selectedEndDate))
            {
                attendancesQuery = attendancesQuery.Where(a => a.Date <= selectedEndDate.Date);
            }

            attendancesQuery = attendancesQuery
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Member.FirstName)
                .ThenBy(a => a.Member.LastName);

            int totalAttendances = await attendancesQuery.CountAsync();

            var attendances = await attendancesQuery
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

            var scheduleRecords = await _context.Schedules
                .Include(s => s.Club)
                .OrderBy(s => s.Club.ClubName)
                .ThenBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            List<SelectListItem> scheduleOptions = new List<SelectListItem>();

            foreach (Schedule schedule in scheduleRecords)
            {
                scheduleOptions.Add(new SelectListItem
                {
                    Value = schedule.ScheduleId.ToString(),
                    Text = schedule.Club.ClubName + " | " + schedule.DayOfWeek + " | " + schedule.StartTime.ToString("hh:mm tt")
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", memberId);
            ViewData["ScheduleId"] = new SelectList(scheduleOptions, "Value", "Text", scheduleId);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalAttendances / (double)pageSize);
            ViewBag.SelectedMemberId = memberId;
            ViewBag.SelectedScheduleId = scheduleId;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(attendances);
        }

        // GET: Attendances/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
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

        // GET: Attendances/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            Attendance attendance = new Attendance();
            attendance.Date = DateTime.Today;

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

            var schedules = await _context.Schedules
                .Include(s => s.Club)
                .OrderBy(s => s.Club.ClubName)
                .ThenBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            List<SelectListItem> scheduleOptions = new List<SelectListItem>();

            foreach (Schedule schedule in schedules)
            {
                scheduleOptions.Add(new SelectListItem
                {
                    Value = schedule.ScheduleId.ToString(),
                    Text = schedule.Club.ClubName + " | " + schedule.DayOfWeek + " | " + schedule.StartTime.ToString("hh:mm tt")
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text");
            ViewData["ScheduleId"] = new SelectList(scheduleOptions, "Value", "Text");

            return View(attendance);
        }

        // POST: Attendances/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttendanceId,ScheduleId,MemberId,Date")] Attendance attendance)
        {
            ModelState.Remove("Member");
            ModelState.Remove("Schedule");

            if (!IsDateBetween1900AndToday(attendance.Date))
            {
                ModelState.AddModelError("Date", "Attendance date must be between 1900 and today.");
            }

            bool attendanceAlreadyExists = await _context.Attendances.AnyAsync(a =>
                a.MemberId == attendance.MemberId &&
                a.ScheduleId == attendance.ScheduleId &&
                a.Date.Date == attendance.Date.Date);

            if (attendanceAlreadyExists)
            {
                ModelState.AddModelError("", "This attendance record already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(attendance);
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

            var schedules = await _context.Schedules
                .Include(s => s.Club)
                .OrderBy(s => s.Club.ClubName)
                .ThenBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            List<SelectListItem> scheduleOptions = new List<SelectListItem>();

            foreach (Schedule schedule in schedules)
            {
                scheduleOptions.Add(new SelectListItem
                {
                    Value = schedule.ScheduleId.ToString(),
                    Text = schedule.Club.ClubName + " | " + schedule.DayOfWeek + " | " + schedule.StartTime.ToString("hh:mm tt")
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", attendance.MemberId);
            ViewData["ScheduleId"] = new SelectList(scheduleOptions, "Value", "Text", attendance.ScheduleId);

            return View(attendance);
        }

        // GET: Attendances/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
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

            var schedules = await _context.Schedules
                .Include(s => s.Club)
                .OrderBy(s => s.Club.ClubName)
                .ThenBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            List<SelectListItem> scheduleOptions = new List<SelectListItem>();

            foreach (Schedule schedule in schedules)
            {
                scheduleOptions.Add(new SelectListItem
                {
                    Value = schedule.ScheduleId.ToString(),
                    Text = schedule.Club.ClubName + " | " + schedule.DayOfWeek + " | " + schedule.StartTime.ToString("hh:mm tt")
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", attendance.MemberId);
            ViewData["ScheduleId"] = new SelectList(scheduleOptions, "Value", "Text", attendance.ScheduleId);

            return View(attendance);
        }

        // POST: Attendances/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttendanceId,ScheduleId,MemberId,Date")] Attendance attendance)
        {
            if (id != attendance.AttendanceId)
            {
                return NotFound();
            }

            ModelState.Remove("Member");
            ModelState.Remove("Schedule");

            if (!IsDateBetween1900AndToday(attendance.Date))
            {
                ModelState.AddModelError("Date", "Attendance date must be between 1900 and today.");
            }

            bool attendanceAlreadyExists = await _context.Attendances.AnyAsync(a =>
                a.MemberId == attendance.MemberId &&
                a.ScheduleId == attendance.ScheduleId &&
                a.Date.Date == attendance.Date.Date &&
                a.AttendanceId != attendance.AttendanceId);

            if (attendanceAlreadyExists)
            {
                ModelState.AddModelError("", "This attendance record already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceExists(attendance.AttendanceId))
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

            var schedules = await _context.Schedules
                .Include(s => s.Club)
                .OrderBy(s => s.Club.ClubName)
                .ThenBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            List<SelectListItem> scheduleOptions = new List<SelectListItem>();

            foreach (Schedule schedule in schedules)
            {
                scheduleOptions.Add(new SelectListItem
                {
                    Value = schedule.ScheduleId.ToString(),
                    Text = schedule.Club.ClubName + " | " + schedule.DayOfWeek + " | " + schedule.StartTime.ToString("hh:mm tt")
                });
            }

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", attendance.MemberId);
            ViewData["ScheduleId"] = new SelectList(scheduleOptions, "Value", "Text", attendance.ScheduleId);

            return View(attendance);
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

        // POST: Attendances/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.AttendanceId == id);
        }

        private bool IsDateBetween1900AndToday(DateTime date)
        {
            DateTime minimumDate = new DateTime(1900, 1, 1);
            DateTime maximumDate = DateTime.Today;

            return date.Date >= minimumDate && date.Date <= maximumDate;
        }
    }
}
