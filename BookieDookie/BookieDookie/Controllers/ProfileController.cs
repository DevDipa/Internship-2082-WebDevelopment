using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;
using BookieDookie.Services.Interface;

namespace BookieDookie.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Edit()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Index", "Login");

            var userId = Guid.Parse(userIdString);

            var user = _userService.GetUserById(userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User updatedUser)
        {
            _userService.UpdateUser(updatedUser);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ToggleStatus(Guid id)
        {
            _userService.ToggleStatus(id);

            return RedirectToAction("Edit");
        }
    }
}