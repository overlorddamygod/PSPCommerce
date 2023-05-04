using System;
using System.ComponentModel.DataAnnotations;

namespace PSPCommerce.Models
{
	public class Product: BaseModel
	{
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        public string ImageUrl { get; set; }
        [Required]
        public string Description { get; set; }
	}
}

