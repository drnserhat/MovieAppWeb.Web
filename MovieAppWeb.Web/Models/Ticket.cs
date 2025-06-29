using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAppWeb.Web.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SeatNumber { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime PurchaseDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPaid { get; set; }
        public string? PaymentReference { get; set; }
        public string? PopcornSize { get; set; }
        public int PopcornQuantity { get; set; }
        public string? Gender { get; set; }

        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
} 