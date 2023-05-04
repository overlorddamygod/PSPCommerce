using System;
using Microsoft.AspNetCore.Identity;

namespace PSPCommerce.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<CartItem> CartItems { get; set; } = default!;
        public virtual ICollection<Favorites> Favorites { get; set; } = default!;
    }
}

