using System.ComponentModel.DataAnnotations;

namespace MovieAppWeb.Web.Models
{
    public class Hall
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int Capacity { get; set; } = 60; // 6 sıra x 10 koltuk = 60

        public int Rows { get; set; } = 6;

        public int SeatsPerRow { get; set; } = 10;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Session>? Sessions { get; set; }

    }
} 