using System.ComponentModel.DataAnnotations;

namespace BookieDookie.Models
{
    public class Book
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Genre { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public Guid UserId { get; set; }

        // Soft deletion
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public Guid? DeletedBy { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}