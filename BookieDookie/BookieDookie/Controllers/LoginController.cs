using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;
using BookieDookie.Services.Interface;

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
        public IActionResult Login(string username, string password)
        {
            var user = _userService.GetUserByUsername(username);

            if (user != null && user.Password == password)
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials!";
            return View("Index");
        }

        [HttpPost]
        public IActionResult SignUp(string email, string username, string password, string confirmPassword)
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
                Password = password
            };

            _userService.AddUser(newUser);

            return RedirectToAction("Index");
        }
    }
}