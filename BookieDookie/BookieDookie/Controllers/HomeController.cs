using System.Diagnostics;
using BookieDookie.Data;
using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;
using BookieDookie.Services.Interface;using BookieDookie.Services.Interface;
using Microsoft.AspNetCore.Authorization;

namespace BookieDookie.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public HomeController(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        public IActionResult Index()
        {
            var user = _context.Users.FirstOrDefault();

            if (user == null)
                return BadRequest("No user found.");

            // 📚 BOOKS READ (books in bookshelf)
            var booksRead = _context.Books
                .Count(b => b.UserId == user.Id);

            // 📊 READING STATS
            var stats = _context.ReadingStats
                .FirstOrDefault(s => s.UserId == user.Id);

            int totalPages = 0;
            int streak = 0;

            if (stats != null)
            {
                totalPages = stats.TotalPagesRead;
                streak = stats.ReadingStreak;

                ViewBag.BookmarkBook = stats.BookmarkBook;
                ViewBag.BookmarkPage = stats.BookmarkPage;
            }

            ViewBag.TotalBooks = booksRead;
            ViewBag.TotalPages = totalPages;
            ViewBag.Streak = streak;

            // Greeting logic (keep your existing one)
            int hour = DateTime.Now.Hour;
            string greeting;

            if (hour >= 5 && hour <= 7)
                greeting = "Goodie morning! Ready to grow today?";
            else if (hour >= 8 && hour <= 11)
                greeting = "Hope your morning is unfolding gently.";
            else if (hour >= 12 && hour <= 15)
                greeting = "Ummhmm...a little afternoon-escape for a few pages, huh?";
            else if (hour >= 16 && hour <= 19)
                greeting = "A quiet evening for reflection.";
            else if (hour >= 20 && hour <= 22)
                greeting = "Want a gentle read before bed?";
            else
                greeting = "Still awake? No worries. BookieDookie's here to befriend you ^.^";

            ViewBag.GreetingLine = greeting;

            return View();
        }
    }

}