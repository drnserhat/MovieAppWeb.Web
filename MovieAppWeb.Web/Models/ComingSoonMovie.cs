using System.ComponentModel.DataAnnotations;

namespace MovieAppWeb.Web.Models
{
    public class ComingSoonMovie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public int Duration { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 