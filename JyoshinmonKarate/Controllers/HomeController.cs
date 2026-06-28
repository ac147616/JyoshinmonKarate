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

            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                string userId = _userManager.GetUserId(User);

                List<Member> allMembers = await _context.Members
                    .Include("Belt")
                    .Include("Club")
                    .ToListAsync();

                foreach (Member member in allMembers)
                {
                    if (member.UserId == userId)
                    {
                        members.Add(member);
                    }
                }
            }

            return View(members);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}