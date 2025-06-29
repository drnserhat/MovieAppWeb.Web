using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAppWeb.Web.Data;
using MovieAppWeb.Web.Models;

namespace MovieAppWeb.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? selectedDate)
    {
        var today = selectedDate?.Date ?? DateTime.Today;

        // Tüm aktif filmleri ve ilişkili seanslarını getir
        var movies = await _context.Movies
            .Include(m => m.Sessions)
            .Where(m => m.IsActive)
            .ToListAsync();

        // Seçilen tarihten önceki seansları ve pasif seansları filtrele
        foreach (var movie in movies)
        {
            movie.Sessions = movie.Sessions
                .Where(s => s.IsActive && s.ShowTime.Date >= today)
                .OrderBy(s => s.ShowTime)
                .ToList();
        }

        // Seansı kalmayan filmleri ele
        movies = movies.Where(m => m.Sessions.Any()).ToList();

        ViewBag.SelectedDate = selectedDate;

        var comingSoonMovies = await _context.ComingSoonMovies
            .Where(m => m.IsActive && m.ReleaseDate >= today)
            .OrderBy(m => m.ReleaseDate)
            .Take(4)
            .ToListAsync();

        ViewBag.ComingSoonMovies = comingSoonMovies;

        return View(movies);
    }

    public async Task<IActionResult> ComingSoon()
    {
        var today = DateTime.Today;
        var comingSoonMovies = await _context.ComingSoonMovies
            .Where(m => m.IsActive && m.ReleaseDate >= today)
            .OrderBy(m => m.ReleaseDate)
            .ToListAsync();

        return View(comingSoonMovies);
    }

    public async Task<IActionResult> ComingSoonDetails(int id)
    {
        var movie = await _context.ComingSoonMovies
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class SpecialOffer
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public int Discount { get; set; }
}

public class ComingSoonMovie
{
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
}
