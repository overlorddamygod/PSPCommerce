using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSPCommerce.Data;
using PSPCommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace PSPCommerce.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AdminController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

            createRoleIfNotExist();
        }

        private async void createRoleIfNotExist()
        {
            string role = "Admin";
            try
            {
                Console.WriteLine("Checking if role " + role + " exists");
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    // Create the role
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));

                    if (result.Succeeded)
                    {
                        // Role created successfully
                        Console.WriteLine("Role created successfully.");
                    }
                    else
                    {
                        // Failed to create the role
                        Console.WriteLine("Failed to create the role:");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR", e);
            }
        }

        [Route("Admin/Add")]
        [Authorize]
        // GET: Cart
        public async Task<IActionResult> Add()
        {
            var user = await _userManager.GetUserAsync(User);

            var role = await _roleManager.FindByNameAsync("Admin");

            if (role == null)
            {
                Console.WriteLine("Role doesn't exist, handle accordingly");
                return NotFound();
            }

            var isInRole = await _userManager.IsInRoleAsync(user, "Admin");

            if (isInRole)
            {
                Console.WriteLine("User is already in role");
                return RedirectToAction("Index", "Home");
            }
            
            Console.WriteLine("Role exists, proceed to add the role to the user");
            var result = await _userManager.AddToRoleAsync(user, "Admin");
            // Role exists, proceed to add the role to the user

            if (!result.Succeeded)
            {
                throw new Exception("Failed to add user to role");
            }
            
            return RedirectToAction("Index", "Home");
        }
    }
}
