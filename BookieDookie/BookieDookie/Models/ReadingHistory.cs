using System.ComponentModel.DataAnnotations;

namespace BookieDookie.Models
{
    public class ReadingHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public DateTime ReadingDate { get; set; }

        public int PagesRead { get; set; }

        public User User { get; set; }
    }
}