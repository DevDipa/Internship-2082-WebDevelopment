using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;

namespace BookieDookie.Controllers
{
    public class ProfileController : Controller
    {
        private static List<User> _users = LoginController._users;

        public IActionResult Edit()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Index", "Login");

            var userId = Guid.Parse(userIdString);

            var user = _users.FirstOrDefault(u => u.Id == userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User updatedUser)
        {
            var user = _users.FirstOrDefault(u => u.Id == updatedUser.Id);

            if (user != null)
            {
                user.Email = updatedUser.Email;
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}