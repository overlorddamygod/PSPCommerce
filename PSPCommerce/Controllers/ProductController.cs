using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSPCommerce.Data;
using PSPCommerce.Models;

namespace PSPCommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: product
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Product.Include(p => p._Category).Include(c => c.Images)
;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p._Category)
                .Include(c => c.Images)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: product/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name");
            return View();
        }

        private async Task<List<ProductImage>> StoreAndGetImages(List<IFormFile> imageFiles)
        {
            List<ProductImage> images = new List<ProductImage>();
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetRandomFileName().Replace(".", string.Empty);
                        var fileExtension = Path.GetExtension(imageFile.FileName);
                        var uniqueFileName = fileName + fileExtension;

                        // Save the image to a specific location
                        var imagePath = Path.Combine("wwwroot/images", uniqueFileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        var imageUrl = Path.Combine("images", uniqueFileName);
                        images.Add(new ProductImage { ImageUrl = imageUrl });
                    }
                }
            }
            return images;
        }

        // POST: product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Price,CategoryID,Description,ID")] Product product, List<IFormFile> imageFiles)
        {
            // log model state errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                StoreAndGetImages(imageFiles).ContinueWith((images) =>
                {
                    product.Images = images.Result;
                }).Wait();

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", product.CategoryID);
            return View(product);
        }


        [HttpGet]
        [Route("product/search")]
        public async Task<IActionResult> Search([FromQuery] ProductSearchParamsDto query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ViewBag.Page = query.Page;
            ViewBag.PageSize = query.PageSize;
            var q = (query.Q ?? String.Empty).ToLower();
            ViewBag.Q = q;
            ViewBag.Category = query.Category;

            ViewBag.MinPrice = query.minPrice;
            ViewBag.MaxPrice = query.maxPrice;

            var products = _context.Product.Where(p => p.Name.ToLower().Contains(q) || p.Description.ToLower().Contains(q))
                .Where(p => p.Price >= query.minPrice && p.Price <= query.maxPrice);

            if (query.Category != null)
            {
                products = products.Where(p => p.CategoryID == query.Category).Include(p => p._Category);
            }

            var paginatedProducts = await products.Include(c => c.Images).Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            ViewBag.TotalPages = Math.Ceiling((double)products.Count() / query.PageSize);

            ViewBag.CategoryID = new SelectList(_context.Category, "ID", "Name");
            return View(paginatedProducts);
        }

        // GET: product/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.Include(p => p.Images).FirstOrDefaultAsync(p => p.ID == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", product.CategoryID);
            return View(product);
        }

        // POST: product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, [Bind("Name,Price,CategoryID,Description,ID")] Product product, List<IFormFile> imageFiles)
        {
            // loop imageFiles
            foreach (IFormFile item in imageFiles)
            {
                Console.WriteLine("HERE", item, "SAD");

            }
            // log model state errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            if (id != product.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var images = await StoreAndGetImages(imageFiles);
                    foreach (ProductImage img in images)
                    {
                        product.Images.Add(img);
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
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
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", product.CategoryID);
            return View(product);
        }

        // GET: product/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p._Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Product?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
