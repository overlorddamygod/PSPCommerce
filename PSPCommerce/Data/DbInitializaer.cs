using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PSPCommerce.Data;
using PSPCommerce.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        // // Ensure the database is created and up to date
        // await context.Database.MigrateAsync();

        // Seed the admin role
        await SeedAdminRole(roleManager);

        // Seed the admin user
        await SeedAdminUser(userManager);
    }

    private static async Task SeedAdminRole(RoleManager<IdentityRole> roleManager)
    {
        string roleName = "Admin";
        bool roleExists = await roleManager.RoleExistsAsync(roleName);
        Console.WriteLine("SEEDING ADMIN ROLE");

        if (!roleExists)
        {
            var role = new IdentityRole(roleName);
            await roleManager.CreateAsync(role);
        }
    }

    private static async Task SeedAdminUser(UserManager<User> userManager)
    {
        string username = "admin@admin.com";
        string email = "admin@admin.com";
        string password = "Admin12345@";

        Console.WriteLine("SEEDING ADMIN USER");

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new User
            {
                UserName = username,
                Email = email,
            };

            var result = await userManager.CreateAsync(user, password);
            Console.WriteLine("CREATED ADMIN USER");

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                // Assign the admin role to the user
                await userManager.AddToRoleAsync(user, "Admin");
                Console.WriteLine("ADDED ROLE TO ADMIN USER");
            }
        }
    }
}
