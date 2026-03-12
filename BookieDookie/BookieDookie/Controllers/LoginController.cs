using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;
using BookieDookie.Services.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BookieDookie.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and password required.";
                return View("Index");
            }

            var user = _userService.GetUserByUsername(username);

            // If user doesn't exist → redirect to signup
            if (user == null)
            {
                return RedirectToAction("Index"); // or show signup section
            }

            // If password wrong
            if (user.Password != password)
            {
                ViewBag.Error = "Invalid password.";
                return View("Index");
            }

            // AUTHENTICATION COOKIE
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal
            );

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SignUp(string email, string username, UserRole role, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View("Index");
            }

            if (_userService.GetUserByUsername(username) != null)
            {
                ViewBag.Error = "Username already exists!";
                return View("Index");
            }

            var newUser = new User
            {
                Email = email,
                Username = username,
                Role = role,
                Password = password
            };

            _userService.AddUser(newUser);

            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }
    }
}