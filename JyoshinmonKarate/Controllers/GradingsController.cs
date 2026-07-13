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
    [Authorize(Roles = "Admin")]
    public class GradingsController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public GradingsController(JyoshinmonKarateContext context)
        {
            _context = context;
        }

        // GET: Gradings
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 20;

            var gradingsQuery = _context.Gradings
                .Include(g => g.Club)
                .OrderByDescending(g => g.GradingDate)
                .ThenBy(g => g.GradingStartTime);

            int totalGradings = await gradingsQuery.CountAsync();

            var gradings = await gradingsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalGradings / (double)pageSize);

            return View(gradings);
        }

        // GET: Gradings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grading = await _context.Gradings
                .Include(g => g.Club)
                .FirstOrDefaultAsync(m => m.GradingId == id);
            if (grading == null)
            {
                return NotFound();
            }

            return View(grading);
        }

        // GET: Gradings/Create
        public IActionResult Create()
        {
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "Address");
            return View();
        }

        // POST: Gradings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GradingId,ClubId,GradingDate,GradingStartTime,GradingEndTime")] Grading grading)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grading);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "Address", grading.ClubId);
            return View(grading);
        }

        // GET: Gradings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grading = await _context.Gradings.FindAsync(id);
            if (grading == null)
            {
                return NotFound();
            }
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "Address", grading.ClubId);
            return View(grading);
        }

        // POST: Gradings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GradingId,ClubId,GradingDate,GradingStartTime,GradingEndTime")] Grading grading)
        {
            if (id != grading.GradingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grading);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradingExists(grading.GradingId))
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
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "Address", grading.ClubId);
            return View(grading);
        }

        // GET: Gradings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grading = await _context.Gradings
                .Include(g => g.Club)
                .FirstOrDefaultAsync(m => m.GradingId == id);
            if (grading == null)
            {
                return NotFound();
            }

            return View(grading);
        }

        // POST: Gradings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grading = await _context.Gradings.FindAsync(id);
            if (grading != null)
            {
                _context.Gradings.Remove(grading);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradingExists(int id)
        {
            return _context.Gradings.Any(e => e.GradingId == id);
        }
    }
}
