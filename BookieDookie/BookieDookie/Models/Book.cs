namespace BookieDookie.Models;

public class Book
{
    public int Id { get; set; }  // unique identifier
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } // optional book image
}