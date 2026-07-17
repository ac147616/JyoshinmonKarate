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
    public class PaymentsController : Controller
    {
        private readonly JyoshinmonKarateContext _context;

        public PaymentsController(JyoshinmonKarateContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index(int memberId = 0, string paymentStatus = "", string paymentMethod = "", string startDate = "", string endDate = "", int page = 1)
        {
            int pageSize = 20;

            var paymentsQuery = _context.Payments
                .Include(p => p.Member)
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

                paymentsQuery = paymentsQuery.Where(p => p.Member.UserId == currentUser.Id);
                membersQuery = membersQuery.Where(m => m.UserId == currentUser.Id);
            }

            if (memberId != 0)
            {
                paymentsQuery = paymentsQuery.Where(p => p.MemberId == memberId);
            }

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                PaymentStatus selectedStatus;

                if (Enum.TryParse(paymentStatus, out selectedStatus))
                {
                    paymentsQuery = paymentsQuery.Where(p => p.Status == selectedStatus);
                }
            }

            if (!string.IsNullOrEmpty(paymentMethod))
            {
                PaymentMethods selectedMethod;

                if (Enum.TryParse(paymentMethod, out selectedMethod))
                {
                    paymentsQuery = paymentsQuery.Where(p => p.PaymentMethod == selectedMethod);
                }
            }

            DateTime selectedStartDate;

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out selectedStartDate))
            {
                paymentsQuery = paymentsQuery.Where(p => p.DateDue >= selectedStartDate.Date);
            }

            DateTime selectedEndDate;

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out selectedEndDate))
            {
                paymentsQuery = paymentsQuery.Where(p => p.DateDue <= selectedEndDate.Date);
            }

            paymentsQuery = paymentsQuery
                .OrderBy(p => p.DateDue)
                .ThenBy(p => p.Member.FirstName)
                .ThenBy(p => p.Member.LastName);

            int totalPayments = await paymentsQuery.CountAsync();

            var payments = await paymentsQuery
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

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", memberId);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPayments / (double)pageSize);
            ViewBag.SelectedMemberId = memberId;
            ViewBag.SelectedPaymentStatus = paymentStatus;
            ViewBag.SelectedPaymentMethod = paymentMethod;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(payments);
        }

        // GET: Payments/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Member)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            Payment payment = new Payment();
            payment.DateDue = DateTime.Today;
            payment.Status = PaymentStatus.Pending;

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

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text");

            return View(payment);
        }

        // POST: Payments/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,MemberId,PaymentName,Amount,DateDue,PaymentMethod,Status")] Payment payment)
        {
            ModelState.Remove("Member");

            if (!IsDateBetween1900And2200(payment.DateDue))
            {
                ModelState.AddModelError("DateDue", "Due date must be between 1900 and 2200.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(payment);
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

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", payment.MemberId);

            return View(payment);
        }

        // GET: Payments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
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

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", payment.MemberId);

            return View(payment);
        }

        // POST: Payments/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,MemberId,PaymentName,Amount,DateDue,PaymentMethod,Status")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            ModelState.Remove("Member");

            if (!IsDateBetween1900And2200(payment.DateDue))
            {
                ModelState.AddModelError("DateDue", "Due date must be between 1900 and 2200.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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

            ViewData["MemberId"] = new SelectList(memberOptions, "Value", "Text", payment.MemberId);

            return View(payment);
        }

        // GET: Payments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Member)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }

        private bool IsDateBetween1900And2200(DateTime date)
        {
            DateTime minimumDate = new DateTime(1900, 1, 1);
            DateTime maximumDate = new DateTime(2200, 1, 1);

            return date.Date >= minimumDate && date.Date <= maximumDate;
        }
    }
}
