using Microsoft.EntityFrameworkCore;
using MovieAppWeb.Web.Models;

namespace MovieAppWeb.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<ComingSoonMovie> ComingSoonMovies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Admin kullanıcısı için seed data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123", // Gerçek uygulamada hash'lenmiş olmalı
                    Email = "admin@movieapp.com",
                    FullName = "Admin User",
                    Phone = "",
                    IsAdmin = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            );
        }
    }
} 