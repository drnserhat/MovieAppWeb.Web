using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieAppWeb.Web.Data;
using MovieAppWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace MovieAppWeb.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .OrderByDescending(s => s.ShowTime)
                .ToListAsync();
            return View(sessions);
        }

        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies.Where(m => m.IsActive), "Id", "Title");
            ViewData["HallId"] = new SelectList(_context.Halls.Where(h => h.IsActive), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Session session)
        {
            if (ModelState.IsValid)
            {
                session.IsActive = true;
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", session.HallId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", session.MovieId);
            return View(session);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            ViewData["MovieId"] = new SelectList(_context.Movies.Where(m => m.IsActive), "Id", "Title", session.MovieId);
            ViewData["HallId"] = new SelectList(_context.Halls.Where(h => h.IsActive), "Id", "Name", session.HallId);
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Session session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.Id))
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
            ViewData["HallId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Halls, "Id", "Name", session.HallId);
            ViewData["MovieId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Movies.Where(m => m.IsActive), "Id", "Title", session.MovieId);
            return View(session);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session != null)
            {
                session.IsActive = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.Id == id);
        }
    }
} 