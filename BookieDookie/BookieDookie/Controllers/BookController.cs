using System.Security.Claims;
using BookieDookie.Models;
using BookieDookie.Services;
using Microsoft.AspNetCore.Mvc;
using BookieDookie.Data;
using BookieDookie.Services.Interface;
using Microsoft.AspNetCore.Authorization;

namespace BookieDookie.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        private readonly ApplicationDbContext _context;

        public BookController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }
        public IActionResult Index()
        {
            var user = _context.Users.FirstOrDefault();

            if (user == null)
                return BadRequest("No user found.");

            var books = _bookService.GetBooksByUser(user.Id);

            return View(books);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book, IFormFile ImageFile)
        {
            var user = _context.Users.FirstOrDefault();

            if (user == null)
                return BadRequest("No user found.");

            if (ImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                book.ImageUrl = "/uploads/" + fileName;
            }

            _bookService.AddBook(book, user.Id);

            return RedirectToAction("Index");
        }

        public IActionResult Edit(Guid id)
        {
            var book = _bookService.GetBookById(id);
            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            _bookService.UpdateBook(book);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Home");
            
            _bookService.DeleteBook(id);
            return RedirectToAction("Index");
        }
    }
}

