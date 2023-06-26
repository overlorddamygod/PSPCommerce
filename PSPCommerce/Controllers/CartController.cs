using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSPCommerce.Data;
using PSPCommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using PSPCommerce.DTO;

namespace PSPCommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public CartController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.CartItem.Where(cart => cart.UserID == user.Id).Include(c => c._Product).Include(c => c._User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Cart/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CartItem == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .Include(c => c._Product)
                .Include(c => c._User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: Cart/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description");
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,Quantity,UserID,ID")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description", cartItem.ProductID);
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id", cartItem.UserID);
            return View(cartItem);
        }

        [HttpPost("/cart/add")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Add(int productId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = user.Id;
            var cartItem = await _context.CartItem.FirstOrDefaultAsync(ci => ci.UserID == userId && ci.ProductID == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductID = productId,
                    Quantity = 1,
                    UserID = userId,
                };

                await _context.CartItem.AddAsync(cartItem);
            }
            else
            {
                cartItem.Quantity += 1;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost("/cart/setquantity")]
        public async Task<IActionResult> SetQuantity([FromBody] SetQuantityDTO data)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            var cartItem = await _context.CartItem.Where(ci => ci.ID == data.id && ci.UserID == user!.Id).FirstOrDefaultAsync();

            if (cartItem != null)
            {
                cartItem.Quantity = data.quantity;
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Cart/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CartItem == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description", cartItem.ProductID);
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id", cartItem.UserID);
            return View(cartItem);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Quantity,UserID,ID")] CartItem cartItem)
        {
            if (id != cartItem.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.ID))
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
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "Description", cartItem.ProductID);
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id", cartItem.UserID);
            return View(cartItem);
        }

        // GET: Cart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CartItem == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .Include(c => c._Product)
                .Include(c => c._User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CartItem == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CartItem'  is null.");
            }
            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(int id)
        {
            return (_context.CartItem?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
