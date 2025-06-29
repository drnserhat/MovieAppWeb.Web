using System.ComponentModel.DataAnnotations;

namespace MovieAppWeb.Web.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Duration { get; set; } // Dakika cinsinden

        [Required]
        public string Genre { get; set; }

        public string? ImagePath { get; set; }

        public DateTime ReleaseDate { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Session>? Sessions { get; set; }
    }
} 