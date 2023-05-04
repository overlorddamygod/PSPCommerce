using System;
using System.ComponentModel.DataAnnotations;

namespace PSPCommerce.Models
{
    public class Product : BaseModel
    {
        [Required]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public double Price { get; set; }
        [Required]
        public string Description { get; set; }
    }
}

