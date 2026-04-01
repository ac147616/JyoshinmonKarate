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
    public class BeltsController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public BeltsController(JyoshinmonKarateContext context)
        {
            _context = context;
        }

        // GET: Belts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Belts.ToListAsync());
        }

        // GET: Belts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var belt = await _context.Belts
                .FirstOrDefaultAsync(m => m.BeltId == id);
            if (belt == null)
            {
                return NotFound();
            }

            return View(belt);
        }

        // GET: Belts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Belts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BeltId,BeltName")] Belt belt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(belt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(belt);
        }

        // GET: Belts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var belt = await _context.Belts.FindAsync(id);
            if (belt == null)
            {
                return NotFound();
            }
            return View(belt);
        }

        // POST: Belts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BeltId,BeltName")] Belt belt)
        {
            if (id != belt.BeltId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(belt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeltExists(belt.BeltId))
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
            return View(belt);
        }

        // GET: Belts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var belt = await _context.Belts
                .FirstOrDefaultAsync(m => m.BeltId == id);
            if (belt == null)
            {
                return NotFound();
            }

            return View(belt);
        }

        // POST: Belts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var belt = await _context.Belts.FindAsync(id);
            if (belt != null)
            {
                _context.Belts.Remove(belt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeltExists(int id)
        {
            return _context.Belts.Any(e => e.BeltId == id);
        }
    }
}
