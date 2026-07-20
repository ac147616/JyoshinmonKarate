using JyoshinmonKarate.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JyoshinmonKarate.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search = "", string role = "", int page = 1)
        {
            int pageSize = 20;

            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                usersQuery = usersQuery.Where(u =>
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search) ||
                    u.Email.Contains(search));
            }

            var allUsers = await usersQuery
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            Dictionary<string, string> userRoles = new Dictionary<string, string>();

            foreach (User user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Count > 0)
                {
                    userRoles[user.Id] = roles[0];
                }
                else
                {
                    userRoles[user.Id] = "No role";
                }
            }

            if (!string.IsNullOrEmpty(role))
            {
                allUsers = allUsers
                    .Where(u => userRoles.ContainsKey(u.Id) && userRoles[u.Id] == role)
                    .ToList();
            }

            int totalUsers = allUsers.Count;

            var users = allUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.UserRoles = userRoles;
            ViewBag.Search = search;
            ViewBag.SelectedRole = role;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            return View(users);
        }


    }
}