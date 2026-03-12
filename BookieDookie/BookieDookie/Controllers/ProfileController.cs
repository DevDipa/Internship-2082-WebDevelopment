using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;
using BookieDookie.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookieDookie.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Edit()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Index", "Login");

            var userId = Guid.Parse(userIdString);

            var user = _userService.GetUserById(userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User updatedUser)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            _userService.UpdateUser(updatedUser);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult DeleteAccount(Guid id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Home");

            _userService.DeleteUser(id);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ToggleStatus(Guid id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            _userService.ToggleStatus(id);

            return RedirectToAction("Edit");
        }
    }
}