using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAppWeb.Web.Data;
using MovieAppWeb.Web.Models;

namespace MovieAppWeb.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dashboardViewModel = new DashboardViewModel
            {
                TotalMovies = _context.Movies.Count(),
                TotalSessions = _context.Sessions.Count(),
                TotalTickets = _context.Tickets.Count(),
                TotalUsers = _context.Users.Count()
            };

            return View(dashboardViewModel);
        }
    }

    public class DashboardViewModel
    {
        public int TotalMovies { get; set; }
        public int TotalSessions { get; set; }
        public int TotalTickets { get; set; }
        public int TotalUsers { get; set; }
    }
} 