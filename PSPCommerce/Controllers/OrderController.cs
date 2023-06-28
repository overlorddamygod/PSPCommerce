using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PSPCommerce.Data;
using PSPCommerce.DTO;
using PSPCommerce.Models;
using Stripe;


namespace PSPCommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        private readonly IConfiguration _config;

        // private readonly StripeConfiguration _stripeConfig;

        public OrderController(ApplicationDbContext context, IConfiguration config, UserManager<User> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            // _stripeConfig = stripeConfig;
        }

        // GET: Order
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var applicationDbContext = _context.Order.Where(order => order.UserID == user.Id).Include(o => o._OrderItems).ThenInclude(o => o._Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Order/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o._User)
                .Include(o => o._OrderItems)
                .ThenInclude(o => o._Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost("/order/create-payment-intent")]
        [Authorize]
        public async Task<ActionResult> CreatePaymentIntent([FromBody] CreateIntentReq req)
        {
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            // get user id from user manager
            var user = await _userManager.GetUserAsync(User);

            // get the total price from the cart
            var cart = _context.CartItem.Where(c => c.UserID == user!.Id).Include(c => c._Product);

            // if cart has no items
            if (cart == null)
            {
                return NotFound();
            }

            // total price of cart items
            int totalPrice = 0;

            foreach (var item in cart)
            {
                totalPrice += item._Product.Price * item.Quantity;
            }

            // _context.Remove(cart);

            var options = new PaymentIntentCreateOptions
            {
                Amount = totalPrice * 100,
                Currency = "npr",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            Order order = new Order();
            order.UserID = user!.Id;
            order.TotalPrice = totalPrice;
            order.IsPaid = false;
            order.PaymentIntentID = intent.Id;
            await _context.AddAsync(order);
            await _context.SaveChangesAsync();

            Console.WriteLine("Order: ", order);

            // add all cart items to order item and delete cart
            foreach (var item in cart)
            {
                OrderItem orderItem = new OrderItem();
                orderItem.OrderID = order.ID;
                orderItem.ProductID = item.ProductID;
                orderItem.Quantity = item.Quantity;
                await _context.AddAsync(orderItem);
                _context.Remove(item);
            }

            await _context.SaveChangesAsync();


            return Json(new { client_secret = intent.ClientSecret });
        }

        [HttpPost("/order/verify-payment")]
        [Authorize]
        public async Task<ActionResult> VerifyPayment([FromBody] VerifyPayReq req)
        {
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(req.paymentId);
            Console.WriteLine("PAYMENT:    ", paymentIntent);
            var order = _context.Order.SingleOrDefault(o => o.PaymentIntentID == req.paymentId);
            if (paymentIntent.Status == "succeeded")
            {
                // payment was successful, update order and order items
                Console.WriteLine("Update ORDER:    ", order);

                if (order != null)
                {
                    order.IsPaid = true;
                    _context.Update(order);
                    Console.WriteLine("Update PAYMENT:    ", paymentIntent);

                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        status = "success",
                        message = "Payment was successful.",
                        redirect_url = "/order/details/" + order.ID + "?success=true&message=Order Received"
                    });
                }
                else
                {
                    return Json(new { status = "error", message = "Order not found.", });
                }
            }
            else
            {
                // payment failed or is still processing, handle accordingly
                if (order != null)
                {
                    return Json(new
                    {
                        status = "error",
                        message = "Payment failed or is still processing.",
                        redirect_url = "/order/details/" + order.ID + "?success=false&message=Payment failed or is still processing."
                    });
                }
                return Json(new
                {
                    status = "error",
                    message = "Payment failed or is still processing.",
                });
            }
        }


        // GET: Order/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,TotalPrice,PaymentIntentID,IsPaid,ID")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id", order.UserID);
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id", order.UserID);
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,TotalPrice,PaymentIntentID,IsPaid,ID")] Order order)
        {
            if (id != order.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.ID))
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
            ViewData["UserID"] = new SelectList(_context.User, "Id", "Id", order.UserID);
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o._User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
