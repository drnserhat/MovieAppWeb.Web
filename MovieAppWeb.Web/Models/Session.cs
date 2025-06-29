using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAppWeb.Web.Models
{
    public class Session
    {
        public int Id { get; set; }

        [Required]
        public DateTime ShowTime { get; set; }

        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie? Movie { get; set; }

        public int HallId { get; set; }

        [ForeignKey("HallId")]
        public virtual Hall? Hall { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
} 