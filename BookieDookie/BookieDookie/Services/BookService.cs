using BookieDookie.Models;
using BookieDookie.Data;

namespace BookieDookie.Services
{

    public class BookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public Book GetBookById(Guid id)
        {
            return _context.Books.FirstOrDefault(b => b.Id == id);
        }

        public void AddBook(Book book, Guid userId)
        {
            book.UserId = userId;

            _context.Books.Add(book);
            _context.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
        }

        public void DeleteBook(Guid id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
        }
    }
}