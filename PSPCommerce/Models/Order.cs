using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSPCommerce.Models
{
    public class Order : BaseModel
    {
        public string UserID { get; set; }
        public int TotalPrice { get; set; }
        public string PaymentIntentID { get; set; }
        public Boolean IsPaid { get; set; }
        public User _User { get; set; }
        public IEnumerable<OrderItem> _OrderItems { get; set; }
    }
}

