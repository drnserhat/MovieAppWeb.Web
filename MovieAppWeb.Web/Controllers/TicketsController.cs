using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAppWeb.Web.Data;
using MovieAppWeb.Web.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Sockets;

namespace MovieAppWeb.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Payment(int id)
        //{
        //    var ticket = await _context.Tickets
        //        .Include(t => t.Session)
        //        .ThenInclude(s => s.Movie)
        //        .Include(t => t.Session)
        //        .ThenInclude(s => s.Hall)
        //        .FirstOrDefaultAsync(t => t.Id == id);

        //    if (ticket == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(ticket);
        //}

        [HttpPost]
        public async Task<IActionResult> Payment(int id, string paymentReference, string cardNumber, string expiryDate, string cvv, string cardHolderName)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.IsPaid)
            {
                return RedirectToAction("Details", new { area = "", id = ticket.Id });
            }

            // Burada gerçek bir ödeme işlemi yapılabilir
            // Şu an için sadece ödeme referansını kaydediyoruz
            ticket.IsPaid = true;
            ticket.PaymentReference = paymentReference;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { area = "", id = ticket.Id });
        }
        public async Task<IActionResult> MyTickets()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound();

            }
            var userId = Convert.ToInt32(User.FindFirst("UserId").Value.ToString());
            var ticket = await _context.Tickets.Where(a=>a.UserId== userId)
                .Include(t => t.Session)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Session)
                    .ThenInclude(s => s.Hall)
                .ToListAsync();

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Session)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Session)
                    .ThenInclude(s => s.Hall)
                .FirstOrDefaultAsync(t => t.SessionId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/SelectSeats/5 (SessionId)
        public async Task<IActionResult> SelectSeats(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            // Fetch existing tickets for this session to mark occupied seats
            var existingTickets = await _context.Tickets
                .Where(t => t.SessionId == id && t.IsActive)
                .ToListAsync();

            ViewBag.OccupiedSeats = existingTickets.Select(t => t.SeatNumber).ToList();

            return View(session);
        }

        // POST: Tickets/Purchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(int sessionId, List<int> selectedSeats, List<string> seatGenders, 
            Dictionary<string, int> popcornQuantities)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login","Account", new { area = "" });
            }
            var userId = Convert.ToInt32(User.FindFirst("UserId").Value.ToString());

            var session = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
            {
                return NotFound();
            }

            // Seçilen koltukların dolu olup olmadığını kontrol et
            var occupiedSeats = await _context.Tickets
                .Where(t => t.SessionId == sessionId && selectedSeats.Contains(t.SeatNumber))
                .Select(t => t.SeatNumber)
                .ToListAsync();

            if (occupiedSeats.Any())
            {
                TempData["Error"] = "Seçilen koltuklardan bazıları dolu!";
                return RedirectToAction("SelectSeats", new {area="", id = sessionId });
            }

            // Toplam mısır fiyatını hesapla
            decimal totalPopcornPrice = 0;
            if (popcornQuantities != null)
            {
                foreach (var popcorn in popcornQuantities)
                {
                    if (popcorn.Value > 0)
                    {
                        switch (popcorn.Key)
                        {
                            case "kucuk":
                                totalPopcornPrice += popcorn.Value * 30;
                                break;
                            case "orta":
                                totalPopcornPrice += popcorn.Value * 45;
                                break;
                            case "buyuk":
                                totalPopcornPrice += popcorn.Value * 60;
                                break;
                        }
                    }
                }
            }

            // Her koltuk için bilet oluştur
            var tickets = new List<Ticket>();
            for (int i = 0; i < selectedSeats.Count; i++)
            {
                var ticket = new Ticket
                {
                    SessionId = sessionId,
                    UserId = userId,
                    SeatNumber = selectedSeats[i],
                    Gender = seatGenders[i],
                    Price = session.Price + (totalPopcornPrice / selectedSeats.Count), // Mısır fiyatını biletlere eşit dağıt
                    PurchaseDate = DateTime.Now,
                    IsActive = true,
                    IsPaid = false // Ödeme yapılmadı olarak işaretle
                };

                // Mısır bilgilerini ekle
                if (popcornQuantities != null && popcornQuantities.Any(kv => kv.Value > 0))
                {
                    var popcornSize = popcornQuantities.FirstOrDefault(kv => kv.Value > 0).Key;
                    ticket.PopcornSize = popcornSize;
                    ticket.PopcornQuantity = popcornQuantities[popcornSize];
                }

                tickets.Add(ticket);
            }

            _context.Tickets.AddRange(tickets);
            await _context.SaveChangesAsync();

            // Tüm biletlerin ID'lerini al ve ödeme sayfasına yönlendir
            return RedirectToAction("Payment", new { area = "", ticketIds = string.Join(",", tickets.Select(t => t.Id)) });
        }

        public async Task<IActionResult> Payment(string ticketIds)
        {
            if (string.IsNullOrEmpty(ticketIds))
            {
                return NotFound();
            }

            var ids = ticketIds.Split(',').Select(int.Parse).ToList();
            var tickets = await _context.Tickets
                .Include(t => t.Session)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Session)
                .ThenInclude(s => s.Hall)
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            if (!tickets.Any())
            {
                return NotFound();
            }

            return View(tickets);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string ticketIds, string cardName, string cardNumber, string expiryDate, string cvv)
        {
            if (string.IsNullOrEmpty(ticketIds))
            {
                return NotFound();
            }

            var ids = ticketIds.Split(',').Select(int.Parse).ToList();
            var tickets = await _context.Tickets
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            if (!tickets.Any())
            {
                return NotFound();
            }

            // Tüm biletleri ödenmiş olarak işaretle
            foreach (var ticket in tickets)
            {
                ticket.IsPaid = true;
                ticket.PaymentReference = Guid.NewGuid().ToString();
            }

            await _context.SaveChangesAsync();

            // İlk biletin ID'sini al ve başarılı ödeme sayfasına yönlendir
            return RedirectToAction("PurchaseSuccess", new { area = "", id = tickets.First().Id });
        }

        public async Task<IActionResult> PurchaseSuccess(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Session)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Session)
                .ThenInclude(s => s.Hall)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
    }
} 