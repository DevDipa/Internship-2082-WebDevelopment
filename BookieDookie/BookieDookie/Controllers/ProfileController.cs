using BookieDookie.Models;
using BookieDookie.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

//Edit profile page

        [HttpGet]
        public IActionResult Edit()
        {
            var userIdString = User.FindFirst("UserId")?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = _userService.GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View(user);
        }

//Update email and username

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            string email,
            string username)
        {
            var userIdString = User.FindFirst("UserId")?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var existingUser = _userService.GetUserById(userId);

            if (existingUser == null)
            {
                return RedirectToAction("Index", "Login");
            }


            //Thoda bahot validation

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username))
            {
                TempData["ProfileError"] =
                    "Email and username are required.";

                return RedirectToAction("Edit");
            }


            //Duplicate names checked
            var usernameUser =
                _userService.GetUserByUsername(username);

            if (usernameUser != null &&
                usernameUser.Id != existingUser.Id)
            {
                TempData["ProfileError"] =
                    "That username is already taken.";

                return RedirectToAction("Edit");
            }


            //Duplicate email 

            var emailUser =
                _userService.GetUserByEmail(email);

            if (emailUser != null &&
                emailUser.Id != existingUser.Id)
            {
                TempData["ProfileError"] =
                    "That email is already registered.";

                return RedirectToAction("Edit");
            }


           //Change

            existingUser.Email = email;
            existingUser.Username = username;

            _userService.UpdateUser(existingUser);


            TempData["ProfileSuccess"] =
                "Profile updated successfully.";

            return RedirectToAction("Edit");
        }

//A/c deletion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdString = User.FindFirst("UserId")?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = _userService.GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }


            // Users are allowed to delete THEIR OWN account.
            _userService.DeleteUser(userId);


            await HttpContext.SignOutAsync();

            HttpContext.Session.Clear();


            return RedirectToAction(
                "Index",
                "Login");
        }


        //Admin le matra garna milne

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(Guid id)
        {
            var targetUser = _userService.GetUserById(id);

            if (targetUser == null)
            {
                return NotFound();
            }

            
            _userService.ToggleStatus(id);


            return RedirectToAction(
                "Edit",
                "Admin");
        }


     //Access Denied means Go back!!!

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}