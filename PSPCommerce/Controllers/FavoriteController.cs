using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSPCommerce.Data;
using PSPCommerce.Models;

namespace PSPCommerce.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoriteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Favorite
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Favorite.Include(f => f._Product).Include(f => f._User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Favorite/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Favorite == null)
            {
                return NotFound();
            }

            var favorites = await _context.Favorite
                .Include(f => f._Product)
                .Include(f => f._User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (favorites == null)
            {
                return NotFound();
            }

            return View(favorites);
        }

        // GET: Favorite/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description");
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Favorite/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,UserID,ID")] Favorites favorites)
        {
            if (ModelState.IsValid)
            {
                _context.Add(favorites);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description", favorites.ProductID);
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id", favorites.UserID);
            return View(favorites);
        }

        // GET: Favorite/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Favorite == null)
            {
                return NotFound();
            }

            var favorites = await _context.Favorite.FindAsync(id);
            if (favorites == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description", favorites.ProductID);
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id", favorites.UserID);
            return View(favorites);
        }

        // POST: Favorite/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,UserID,ID")] Favorites favorites)
        {
            if (id != favorites.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favorites);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoritesExists(favorites.ID))
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
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description", favorites.ProductID);
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id", favorites.UserID);
            return View(favorites);
        }

        // GET: Favorite/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Favorite == null)
            {
                return NotFound();
            }

            var favorites = await _context.Favorite
                .Include(f => f._Product)
                .Include(f => f._User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (favorites == null)
            {
                return NotFound();
            }

            return View(favorites);
        }

        // POST: Favorite/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Favorite == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Favorite'  is null.");
            }
            var favorites = await _context.Favorite.FindAsync(id);
            if (favorites != null)
            {
                _context.Favorite.Remove(favorites);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoritesExists(int id)
        {
          return (_context.Favorite?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}