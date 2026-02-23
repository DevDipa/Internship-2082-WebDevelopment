namespace BookieDookie.Models;

public class BookViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public IFormFile? Image { get; set; } // optional file upload
}