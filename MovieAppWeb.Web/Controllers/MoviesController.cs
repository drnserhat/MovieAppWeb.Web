using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAppWeb.Web.Data;
using MovieAppWeb.Web.Models;

namespace MovieAppWeb.Web.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Sessions(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Sessions)
                    .ThenInclude(s => s.Hall)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (movie == null)
            {
                return NotFound();
            }

            var today = DateTime.Today;
            var sessions = movie.Sessions
                .Where(s => s.ShowTime.Date >= today && s.IsActive)
                .OrderBy(s => s.ShowTime)
                .ToList();

            ViewBag.Movie = movie;
            return View(sessions);
        }

        public async Task<IActionResult> Book(int id)
        {
            var session = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (session == null)
            {
                return NotFound();
            }

            if (session.ShowTime < DateTime.Now)
            {
                return BadRequest("Bu seans için bilet satışı sona ermiştir.");
            }

            var bookedSeats = session.Tickets.Select(t => t.SeatNumber).ToList();
            ViewBag.BookedSeats = bookedSeats;
            ViewBag.TotalSeats = session.Hall.Capacity;

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> Book(int id, int seatNumber)
        {
            var session = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (session == null)
            {
                return NotFound();
            }

            if (session.ShowTime < DateTime.Now)
            {
                return BadRequest("Bu seans için bilet satışı sona ermiştir.");
            }

            if (session.Tickets.Any(t => t.SeatNumber == seatNumber))
            {
                return BadRequest("Bu koltuk dolu.");
            }

            if (seatNumber < 1 || seatNumber > session.Hall.Capacity)
            {
                return BadRequest("Geçersiz koltuk numarası.");
            }

            var ticket = new Ticket
            {
                SessionId = session.Id,
                SeatNumber = seatNumber,
                Price = session.Price,
                PurchaseDate = DateTime.Now,
                IsPaid = false
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("Payment", "Tickets", new { id = ticket.Id });
        }
    }
} 