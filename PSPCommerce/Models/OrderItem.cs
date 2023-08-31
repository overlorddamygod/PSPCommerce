using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSPCommerce.Models
{
    public class OrderItem : BaseModel
    {
        public int Quantity { get; set; }
        public int ProductID { get; set; }
        public int OrderID { get; set; }

        public Order? _Order { get; set; } = null!;
        public Product? _Product { get; set; } = null!;
    }
}

