using System;
using System.ComponentModel.DataAnnotations;

namespace PSPCommerce.Models
{
    public class Product : BaseModel
    {
        [Required]
        public string Name { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int Price { get; set; }
        public int CategoryID { get; set; }
        public Category? _Category { get; set; } = null!; 

        [Required]
        public string Description { get; set; }
    }
}

