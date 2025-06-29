using System.ComponentModel.DataAnnotations;

namespace MovieAppWeb.Web.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta adresi en fazla 100 karakter olabilir.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, ErrorMessage = "Şifre en fazla 100 karakter olabilir.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Ad soyad zorunludur.")]
        [StringLength(100, ErrorMessage = "Ad soyad en fazla 100 karakter olabilir.")]
        public string FullName { get; set; }

        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir.")]
        public string Phone { get; set; }

        public bool IsAdmin { get; set; }
        public bool Gender { get; set; } // false ise erkek true ise kadın
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
} 