using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSPCommerce.Data;
using PSPCommerce.Models;
using Microsoft.AspNetCore.Authorization;

namespace PSPCommerce.Controllers
{

    [Authorize(Roles = "Admin")]
    public class ProductImageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductImageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductImage
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductImage.Include(p => p._Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductImage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductImage == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImage
                .Include(p => p._Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // GET: ProductImage/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description");
            return View();
        }

        // POST: ProductImage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ImageUrl,ID,CreatedAt")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description", productImage.ProductID);
            return View(productImage);
        }

        // GET: ProductImage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductImage == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImage.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description", productImage.ProductID);
            return View(productImage);
        }

        // POST: ProductImage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ImageUrl,ID,CreatedAt")] ProductImage productImage)
        {
            if (id != productImage.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.ID))
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
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description", productImage.ProductID);
            return View(productImage);
        }

        // GET: ProductImage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductImage == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImage
                .Include(p => p._Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: ProductImage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductImage == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductImage'  is null.");
            }
            var productImage = await _context.ProductImage.FindAsync(id);
            if (productImage != null)
            {
                _context.ProductImage.Remove(productImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteImage(int id)
        {
            Console.WriteLine("DELETE IMG");
            if (_context.ProductImage == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductImage'  is null.");
            }
            var productImage = await _context.ProductImage.FindAsync(id);
            if (productImage != null)
            {
                _context.ProductImage.Remove(productImage);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Product", new { id = productImage!.ProductID });
        }

        private bool ProductImageExists(int id)
        {
            return (_context.ProductImage?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
