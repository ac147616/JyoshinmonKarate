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
    public class MemberMembershipsController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public MemberMembershipsController(JyoshinmonKarateContext context)
        {
            _context = context;
        }

        // GET: MemberMemberships
        public async Task<IActionResult> Index()
        {
            var jyoshinmonKarateContext = _context.MemberMemberships.Include(m => m.Member).Include(m => m.Membership);
            return View(await jyoshinmonKarateContext.ToListAsync());
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
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "EmergencyContactName");
            ViewData["MembershipId"] = new SelectList(_context.Memberships, "MembershipId", "AgeGroup");
            return View();
        }

        // POST: MemberMemberships/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
