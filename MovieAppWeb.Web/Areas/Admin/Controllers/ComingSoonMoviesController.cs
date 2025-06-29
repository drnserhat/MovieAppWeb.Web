using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieAppWeb.Web.Data;
using MovieAppWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using MovieAppWeb.Web.Helpers;

namespace MovieAppWeb.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ComingSoonMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComingSoonMoviesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.ComingSoonMovies
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
            return View(movies);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComingSoonMovie movie, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                  

                    movie.ImagePath = UploadFile.Upload(imageFile);

                }

                _context.Add(movie);
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

            var movie = await _context.ComingSoonMovies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComingSoonMovie movie, IFormFile? imageFile)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMovie = await _context.ComingSoonMovies.FindAsync(id);
                    if (imageFile != null)
                    {

                        if (existingMovie.ImagePath != null)
                        {
                            string webRootPath = _webHostEnvironment.WebRootPath;
                            string oldFilePath = Path.Combine(webRootPath, "files", existingMovie.ImagePath);

                            if (System.IO.File.Exists(oldFilePath))
                            {
                                try
                                {
                                    // Dosya kullanýmda mý kontrol et
                                    using (FileStream stream = new FileStream(oldFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                                    {
                                        // dosyaya eriþilebiliyor, kapatýlýp silinecek
                                    }

                                    System.IO.File.Delete(oldFilePath);
                                }
                                catch (IOException ex)
                                {
                                    // Hata loglanabilir ya da kullanýcý bilgilendirilebilir
                                    ModelState.AddModelError("", "Resim dosyasý silinemedi. Dosya baþka bir iþlem tarafýndan kullanýlýyor.");
                                }
                            }
                        }



                        existingMovie.ImagePath = UploadFile.Upload(imageFile);
                    }
                  

                    existingMovie.Title = movie.Title;
                    existingMovie.Description = movie.Description;
                    existingMovie.ReleaseDate = movie.ReleaseDate;
                    existingMovie.Genre = movie.Genre;
                    existingMovie.Duration = movie.Duration;
                    existingMovie.IsActive = movie.IsActive;
                    existingMovie.CreatedAt = movie.CreatedAt;

                    _context.Update(existingMovie);
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
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.ComingSoonMovies
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
            var movie = await _context.ComingSoonMovies.FindAsync(id);
            if (movie != null)
            {
                movie.IsActive = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.ComingSoonMovies.Any(e => e.Id == id);
        }
    }
} 