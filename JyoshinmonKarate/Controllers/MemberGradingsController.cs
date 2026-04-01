using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JyoshinmonKarate.Areas.Identity.Data;
using JyoshinmonKarate.Models;

namespace JyoshinmonKarate.Controllers
{
    public class MemberGradingsController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public MemberGradingsController(JyoshinmonKarateContext context)
        {
            _context = context;
        }

        // GET: MemberGradings
        public async Task<IActionResult> Index()
        {
            var jyoshinmonKarateContext = _context.MemberGradings.Include(m => m.BeltAfter).Include(m => m.BeltBefore).Include(m => m.Grading).Include(m => m.Member);
            return View(await jyoshinmonKarateContext.ToListAsync());
        }

        // GET: MemberGradings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberGrading = await _context.MemberGradings
                .Include(m => m.BeltAfter)
                .Include(m => m.BeltBefore)
                .Include(m => m.Grading)
                .Include(m => m.Member)
                .FirstOrDefaultAsync(m => m.MemberGradingId == id);
            if (memberGrading == null)
            {
                return NotFound();
            }

            return View(memberGrading);
        }

        // GET: MemberGradings/Create
        public IActionResult Create()
        {
            ViewData["BeltAfterId"] = new SelectList(_context.Belts, "BeltId", "BeltName");
            ViewData["BeltBeforeId"] = new SelectList(_context.Belts, "BeltId", "BeltName");
            ViewData["GradingId"] = new SelectList(_context.Gradings, "GradingId", "GradingId");
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "FirstName");
            return View();
        }

        // POST: MemberGradings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberGradingId,GradingId,MemberId,BeltBeforeId,BeltAfterId,Passed")] MemberGrading memberGrading)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberGrading);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BeltAfterId"] = new SelectList(_context.Belts, "BeltId", "BeltName", memberGrading.BeltAfterId);
            ViewData["BeltBeforeId"] = new SelectList(_context.Belts, "BeltId", "BeltName", memberGrading.BeltBeforeId);
            ViewData["GradingId"] = new SelectList(_context.Gradings, "GradingId", "GradingId", memberGrading.GradingId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "FirstName", memberGrading.MemberId);
            return View(memberGrading);
        }

        // GET: MemberGradings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberGrading = await _context.MemberGradings.FindAsync(id);
            if (memberGrading == null)
            {
                return NotFound();
            }
            ViewData["BeltAfterId"] = new SelectList(_context.Belts, "BeltId", "BeltName", memberGrading.BeltAfterId);
            ViewData["BeltBeforeId"] = new SelectList(_context.Belts, "BeltId", "BeltName", memberGrading.BeltBeforeId);
            ViewData["GradingId"] = new SelectList(_context.Gradings, "GradingId", "GradingId", memberGrading.GradingId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "FirstName", memberGrading.MemberId);
            return View(memberGrading);
        }

        // POST: MemberGradings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberGradingId,GradingId,MemberId,BeltBeforeId,BeltAfterId,Passed")] MemberGrading memberGrading)
        {
            if (id != memberGrading.MemberGradingId)
            {
                return NotFound();
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
                    if (!MemberGradingExists(memberGrading.MemberGradingId))
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
            ViewData["BeltAfterId"] = new SelectList(_context.Belts, "BeltId", "BeltName", memberGrading.BeltAfterId);
            ViewData["BeltBeforeId"] = new SelectList(_context.Belts, "BeltId", "BeltName", memberGrading.BeltBeforeId);
            ViewData["GradingId"] = new SelectList(_context.Gradings, "GradingId", "GradingId", memberGrading.GradingId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "FirstName", memberGrading.MemberId);
            return View(memberGrading);
        }

        // GET: MemberGradings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberGrading = await _context.MemberGradings
                .Include(m => m.BeltAfter)
                .Include(m => m.BeltBefore)
                .Include(m => m.Grading)
                .Include(m => m.Member)
                .FirstOrDefaultAsync(m => m.MemberGradingId == id);
            if (memberGrading == null)
            {
                return NotFound();
            }

            return View(memberGrading);
        }

        // POST: MemberGradings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberGrading = await _context.MemberGradings.FindAsync(id);
            if (memberGrading != null)
            {
                _context.MemberGradings.Remove(memberGrading);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberGradingExists(int id)
        {
            return _context.MemberGradings.Any(e => e.MemberGradingId == id);
        }
    }
}
