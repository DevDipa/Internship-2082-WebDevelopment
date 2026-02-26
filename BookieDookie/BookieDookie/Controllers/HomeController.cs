using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;
using BookieDookie.Services.Interface;using BookieDookie.Services.Interface;

namespace BookieDookie.Controllers
{

    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            ViewBag.ActiveUsers = _userService.GetActiveUsers();
            ViewBag.InactiveUsers = _userService.GetInactiveUsers();

            return View();
        }
    }

}