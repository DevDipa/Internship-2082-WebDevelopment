namespace BookieDookie.Models
{
    public class ReadingStats
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public int PagesReadToday { get; set; }

        public int TotalPagesRead { get; set; }

        public int BooksRead { get; set; }

        public int ReadingStreak { get; set; }

        public string? Feeling { get; set; }

        public string? BookmarkBook { get; set; }

        public int BookmarkPage { get; set; }

        public DateTime LastUpdated { get; set; }

        public User User { get; set; }
    }
}