using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSPCommerce.Models
{
    public class CartItem : BaseModel
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string UserID { get; set; }

        public User _User { get; set; } = null!; 

        public Product _Product { get; set; } = null!; 
    }
}

