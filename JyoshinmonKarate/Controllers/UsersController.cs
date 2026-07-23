using JyoshinmonKarate.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JyoshinmonKarate.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JyoshinmonKarateContext _context;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, JyoshinmonKarateContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
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

        // GET: Users/Details/id
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0)
            {
                ViewBag.UserRole = roles[0];
            }
            else
            {
                ViewBag.UserRole = "No role";
            }

            return View(user);
        }

        // GET: Users/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            string selectedRole = "";

            if (currentRoles.Count > 0)
            {
                selectedRole = currentRoles[0];
            }

            ViewData["Roles"] = new SelectList(
                _roleManager.Roles.OrderBy(r => r.Name),
                "Name",
                "Name",
                selectedRole);

            return View(user);
        }

        // POST: Users/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Email,PhoneNumber")] User user, string selectedRole)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            User existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            ModelState.Remove("UserName");
            ModelState.Remove("NormalizedUserName");
            ModelState.Remove("NormalizedEmail");
            ModelState.Remove("PasswordHash");
            ModelState.Remove("SecurityStamp");
            ModelState.Remove("ConcurrencyStamp");
            ModelState.Remove("Members");
            ModelState.Remove("Instructor");

            if (!ModelState.IsValid)
            {
                ViewData["Roles"] = new SelectList(
                    _roleManager.Roles.OrderBy(r => r.Name),
                    "Name",
                    "Name",
                    selectedRole);

                return View(user);
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.UserName = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;

            IdentityResult updateResult = await _userManager.UpdateAsync(existingUser);

            if (!updateResult.Succeeded)
            {
                foreach (IdentityError error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ViewData["Roles"] = new SelectList(
                    _roleManager.Roles.OrderBy(r => r.Name),
                    "Name",
                    "Name",
                    selectedRole);

                return View(user);
            }

            var currentRoles = await _userManager.GetRolesAsync(existingUser);

            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
            }

            if (!string.IsNullOrEmpty(selectedRole))
            {
                await _userManager.AddToRoleAsync(existingUser, selectedRole);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            bool hasMembers = await _context.Members.AnyAsync(m => m.UserId == user.Id);
            bool isInstructor = await _context.Instructors.AnyAsync(i => i.UserId == user.Id);

            ViewBag.CanDelete = !hasMembers && !isInstructor;
            ViewBag.HasMembers = hasMembers;
            ViewBag.IsInstructor = isInstructor;

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0)
            {
                ViewBag.UserRole = roles[0];
            }
            else
            {
                ViewBag.UserRole = "No role";
            }

            return View(user);
        }

        // POST: Users/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            bool hasMembers = await _context.Members.AnyAsync(m => m.UserId == user.Id);
            bool isInstructor = await _context.Instructors.AnyAsync(i => i.UserId == user.Id);

            if (hasMembers || isInstructor)
            {
                TempData["ErrorMessage"] = "This account cannot be deleted because it is linked to existing member or instructor records.";
                return RedirectToAction(nameof(Index));
            }

            IdentityResult deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                foreach (IdentityError error in deleteResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ViewBag.CanDelete = true;

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Count > 0)
                {
                    ViewBag.UserRole = roles[0];
                }
                else
                {
                    ViewBag.UserRole = "No role";
                }

                return View("Delete", user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}