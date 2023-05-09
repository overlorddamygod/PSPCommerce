using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PSPCommerce.Models;

namespace PSPCommerce.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Product { get; set; } = default!;
    public DbSet<CartItem> CartItem { get; set; } = default!;
    public DbSet<Favorites> Favorite { get; set; } = default!;
    public DbSet<Category> Category { get; set; } = default!;
    public DbSet<User> User { get; set; } = default!;
    public DbSet<Order> Order { get; set; } = default!;
    public DbSet<OrderItem> OrderItem { get; set; } = default!;
}

