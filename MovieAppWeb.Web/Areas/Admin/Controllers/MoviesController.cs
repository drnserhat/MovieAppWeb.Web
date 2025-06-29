using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAppWeb.Web.Data;
using MovieAppWeb.Web.Helpers;
using MovieAppWeb.Web.Models;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;

namespace MovieAppWeb.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MoviesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                System.GC.Collect();
                string webRootPath = _env.WebRootPath;

                if (movie.ImagePath == "" || movie.ImagePath == null)
                {
                    movie.ImagePath = UploadFile.Upload(ImageFile);
                }
                else if (movie != null)
                {
                    string oldFilePath = Path.Combine(webRootPath, "files", movie.ImagePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    movie.ImagePath = UploadFile.Upload(ImageFile);
                }
                await _context.AddAsync(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? ImageFile)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMovie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (ImageFile != null)
                    {
                        string webRootPath = _env.WebRootPath;
                        string oldFilePath = Path.Combine(webRootPath, "files", existingMovie.ImagePath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }

                        movie.ImagePath = UploadFile.Upload(ImageFile);
                    }
                    else
                    {
                        movie.ImagePath = existingMovie.ImagePath;
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var movieModel = await _context.Movies.FindAsync(id);
            if (movie.ImagePath==null)
            {
                movie.ImagePath = movieModel.ImagePath;
            }
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                if (!string.IsNullOrEmpty(movie.ImagePath))
                {
                    string webRootPath = _env.WebRootPath;
                    string filePath = Path.Combine(webRootPath, "files", movie.ImagePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                movie.IsActive = false;
                 _context.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
} 