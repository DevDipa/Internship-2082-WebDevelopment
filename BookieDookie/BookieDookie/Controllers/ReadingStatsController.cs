using BookieDookie.Data;
using BookieDookie.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookieDookie.Controllers
{

    public class ReadingStatsController : Controller {
        
        private readonly ApplicationDbContext _context;

        public ReadingStatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var user = _context.Users.FirstOrDefault();

            if (user == null)
            {
                return Content("No user found in database.");
            }

            var stats = _context.ReadingStats
                .FirstOrDefault(s => s.UserId == user.Id);

            if (stats == null)
            {
                stats = new ReadingStats
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    PagesReadToday = 0,
                    TotalPagesRead = 0,
                    BooksRead = 0,
                    ReadingStreak = 0,
                    LastUpdated = DateTime.UtcNow
                };

                _context.ReadingStats.Add(stats);
                _context.SaveChanges();
            }

            return View(stats);
        }

        [HttpPost]
        public IActionResult UpdatePages(int pages)
        {
            var user = _context.Users.FirstOrDefault();

            var stats = _context.ReadingStats
                .FirstOrDefault(s => s.UserId == user.Id);

            stats.PagesReadToday = pages;
            stats.TotalPagesRead += pages;
            stats.LastUpdated = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public IActionResult UpdateBookmark(string book, int page)
        {
            var user = _context.Users.FirstOrDefault();

            var stats = _context.ReadingStats
                .FirstOrDefault(s => s.UserId == user.Id);

            stats.BookmarkBook = book;
            stats.BookmarkPage = page;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public IActionResult UpdateFeeling(string feeling)
        {
            var user = _context.Users.FirstOrDefault();

            var stats = _context.ReadingStats
                .FirstOrDefault(s => s.UserId == user.Id);

            stats.Feeling = feeling;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}