using BookieDookie.Models;

namespace BookieDookie.Services.Interface
{
    public interface IBookService
    {
        List<Book> GetAllBooks();

        Book GetBookById(Guid id);

        List<Book> GetBooksByUser(Guid userId);

        void AddBook(Book book, Guid userId);

        void UpdateBook(Book book);

        void DeleteBook(Guid id);
    }
}