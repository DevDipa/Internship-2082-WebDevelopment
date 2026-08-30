using BookieDookie.Data;
using BookieDookie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookieDookie.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // ADMIN DASHBOARD
        // ==========================================

        [HttpGet]
        public IActionResult Index()
        {
            // =========================
            // USERS
            // =========================

            var totalUsers = _context.Users
                .Count(u => !u.IsDeleted);

            var activeUsers = _context.Users
                .Count(u =>
                    !u.IsDeleted &&
                    u.Status == UserStatus.Active);

            var inactiveUsers = _context.Users
                .Count(u =>
                    !u.IsDeleted &&
                    u.Status == UserStatus.Inactive);

            // =========================
            // BOOKS
            // =========================

            var totalBooks = _context.Books
                .Count(b => !b.IsDeleted);

            // =========================
            // READING STATISTICS
            // =========================

            var totalPagesRead = _context.ReadingStats
                .Sum(s => (int?)s.TotalPagesRead) ?? 0;

            var averageStreak = _context.ReadingStats
                .Any()
                ? Math.Round(
                    _context.ReadingStats
                        .Average(s => (double)s.ReadingStreak),
                    1)
                : 0;

            // =========================
            // ADMIN INFORMATION
            // =========================

            var adminUsername =
                User.Identity?.Name ?? "Administrator";

            // =========================
            // USER OVERVIEW
            // =========================

            var users = _context.Users
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.Role)
                .ThenBy(u => u.Username)
                .Select(u => new AdminUserViewModel
                {
                    UserId = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Status = u.Status,
                    Role = u.Role,

                    BookCount = _context.Books
                        .Count(b =>
                            b.UserId == u.Id &&
                            !b.IsDeleted),

                    PagesRead = _context.ReadingStats
                        .Where(s => s.UserId == u.Id)
                        .Select(s => (int?)s.TotalPagesRead)
                        .FirstOrDefault() ?? 0,

                    ReadingStreak = _context.ReadingStats
                        .Where(s => s.UserId == u.Id)
                        .Select(s => (int?)s.ReadingStreak)
                        .FirstOrDefault() ?? 0
                })
                .ToList();

            // =========================
            // VIEW BAG DATA
            // =========================

            ViewBag.TotalUsers = totalUsers;
            ViewBag.ActiveUsers = activeUsers;
            ViewBag.InactiveUsers = inactiveUsers;

            ViewBag.TotalBooks = totalBooks;
            ViewBag.TotalPagesRead = totalPagesRead;
            ViewBag.AverageStreak = averageStreak;

            ViewBag.AdminUsername = adminUsername;

            return View(users);
        }
        
// ==========================================
// ADMIN USERS
// ==========================================

[HttpGet]
public IActionResult Users()
{
    var users = _context.Users
        .Where(u => !u.IsDeleted)
        .OrderBy(u => u.Role)
        .ThenBy(u => u.Username)
        .Select(u => new AdminUserViewModel
        {
            UserId = u.Id,
            Username = u.Username,
            Email = u.Email,
            Status = u.Status,
            Role = u.Role,

            BookCount = _context.Books
                .Count(b =>
                    b.UserId == u.Id &&
                    !b.IsDeleted),

            PagesRead = _context.ReadingStats
                .Where(s => s.UserId == u.Id)
                .Select(s => (int?)s.TotalPagesRead)
                .FirstOrDefault() ?? 0,

            ReadingStreak = _context.ReadingStats
                .Where(s => s.UserId == u.Id)
                .Select(s => (int?)s.ReadingStreak)
                .FirstOrDefault() ?? 0
        })
        .ToList();

    return View(users);
}


// ==========================================
// ACTIVATE / DEACTIVATE USER
// ==========================================

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult ToggleUserStatus(Guid id)
{
    var user = _context.Users
        .FirstOrDefault(u =>
            u.Id == id &&
            !u.IsDeleted);

    if (user == null)
        return NotFound();

    // Prevent Admin from accidentally disabling themselves
    if (user.Role == UserRole.Admin &&
        User.Identity?.Name == user.Username)
    {
        TempData["AdminError"] = "You cannot deactivate your own administrator account.";
        return RedirectToAction("Users");
    }

    if (user.Status == UserStatus.Active)
        user.Status = UserStatus.Inactive;
    else
        user.Status = UserStatus.Active;

    _context.SaveChanges();

    return RedirectToAction("Users");
}


// ==========================================
// DELETE USER
// ==========================================

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult DeleteUser(Guid id)
{
    var user = _context.Users
        .FirstOrDefault(u =>
            u.Id == id &&
            !u.IsDeleted);

    if (user == null)
        return NotFound();

    // Prevent Admin from deleting themselves
    if (user.Role == UserRole.Admin &&
        User.Identity?.Name == user.Username)
    {
        TempData["AdminError"] = "You cannot delete your own administrator account.";
        return RedirectToAction("Users");
    }

    // Soft delete
    user.IsDeleted = true;
    user.DeletedAt = DateTime.UtcNow;

    _context.SaveChanges();

    return RedirectToAction("Users");
}
}


    // ==========================================
    // ADMIN USER VIEW MODEL
    // ==========================================
    
    public class AdminUserViewModel
    {
        public Guid UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserStatus Status { get; set; }

        public UserRole Role { get; set; }

        public int BookCount { get; set; }

        public int PagesRead { get; set; }

        public int ReadingStreak { get; set; }
    }
    
}

