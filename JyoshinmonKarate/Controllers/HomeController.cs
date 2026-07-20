using System.Diagnostics;
using JyoshinmonKarate.Areas.Identity.Data;
using JyoshinmonKarate.Data;
using JyoshinmonKarate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace JyoshinmonKarate.Controllers
{
    public class HomeController : Controller
    {
        private readonly JyoshinmonKarateContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(JyoshinmonKarateContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<Member> members = new List<Member>();

            Dictionary<int, string> membershipNames = new Dictionary<int, string>();
            Dictionary<int, string> lastClassDates = new Dictionary<int, string>();
            Dictionary<int, decimal> outstandingPayments = new Dictionary<int, decimal>();

            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                string userId = _userManager.GetUserId(User);

                List<Member> allMembers = await _context.Members
                    .Include("Belt")
                    .ToListAsync();

                foreach (Member member in allMembers)
                {
                    if (member.UserId == userId)
                    {
                        members.Add(member);
                    }
                }

                List<MemberMembership> allMemberships = await _context.MemberMemberships
                    .Include("Membership")
                    .ToListAsync();

                foreach (Member member in members)
                {
                    string membershipName = "Not assigned";
                    DateTime latestStartDate = DateTime.MinValue;

                    foreach (MemberMembership memberMembership in allMemberships)
                    {
                        if (memberMembership.MemberId == member.MemberId)
                        {
                            if (memberMembership.StartDate > latestStartDate)
                            {
                                latestStartDate = memberMembership.StartDate;

                                if (memberMembership.Membership != null)
                                {
                                    membershipName = memberMembership.Membership.MembershipName;
                                }
                            }
                        }
                    }

                    membershipNames[member.MemberId] = membershipName;
                }

                List<Attendance> allAttendances = await _context.Attendances.ToListAsync();

                foreach (Member member in members)
                {
                    string lastClass = "No attendance yet";
                    DateTime latestDate = DateTime.MinValue;

                    foreach (Attendance attendance in allAttendances)
                    {
                        if (attendance.MemberId == member.MemberId)
                        {
                            if (attendance.Date > latestDate)
                            {
                                latestDate = attendance.Date;
                                lastClass = attendance.Date.ToString("dd MMM yyyy");
                            }
                        }
                    }

                    lastClassDates[member.MemberId] = lastClass;
                }

                List<Payment> allPayments = await _context.Payments.ToListAsync();

                foreach (Member member in members)
                {
                    decimal totalDue = 0;

                    foreach (Payment payment in allPayments)
                    {
                        if (payment.MemberId == member.MemberId)
                        {
                            if (payment.Status.ToString() != "Paid")
                            {
                                totalDue = totalDue + payment.Amount;
                            }
                        }
                    }

                    outstandingPayments[member.MemberId] = totalDue;
                }
            }

            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                ViewBag.TotalMembers = await _context.Members.CountAsync();
                ViewBag.TotalInstructors = await _context.Instructors.CountAsync();
                ViewBag.TotalDojos = await _context.Clubs.CountAsync();

                ViewBag.ActiveMemberships = await _context.MemberMemberships
                    .CountAsync(m => m.MembershipStatus == MembershipStatus.Active);

                ViewBag.PendingPayments = await _context.Payments
                    .CountAsync(p => p.Status == PaymentStatus.Pending);

                ViewBag.FailedPayments = await _context.Payments
                    .CountAsync(p => p.Status == PaymentStatus.Failed);

                ViewBag.OutstandingPaymentTotal = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Failed)
                    .SumAsync(p => (decimal?)p.Amount) ?? 0;

                ViewBag.UpcomingGradings = await _context.Gradings
                    .CountAsync(g => g.GradingDate >= DateTime.Today);

                ViewBag.RecentMembers = await _context.Members
                    .Include(m => m.Belt)
                    .Include(m => m.Club)
                    .OrderByDescending(m => m.DateJoined)
                    .Take(5)
                    .ToListAsync();
            }

            ViewBag.MembershipNames = membershipNames;
            ViewBag.LastClassDates = lastClassDates;
            ViewBag.OutstandingPayments = outstandingPayments;

            return View(members);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}