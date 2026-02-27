using BookieDookie.Models;
using BookieDookie.Services;
using Microsoft.AspNetCore.Mvc;
using BookieDookie.Data;

namespace BookieDookie.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _bookService;

        private readonly ApplicationDbContext _context;

        public BookController(BookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        public IActionResult Index()
        {
            var books = _bookService.GetAllBooks();
            return View(books);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            var user = _context.Users.FirstOrDefault();

            if (user == null)
                return BadRequest("No user found.");

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

        public IActionResult Delete(Guid id)
        {
            _bookService.DeleteBook(id);
            return RedirectToAction("Index");
        }
    }
}

