using System.ComponentModel.DataAnnotations;

namespace PSPCommerce.DTO;

public class SetQuantityDTO{
    public int id { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int quantity { get; set; }
}