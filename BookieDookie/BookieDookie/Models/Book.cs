using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        //navigation property that lets EF Core automatically load the related user
        public User User { get; set; }
    }
}