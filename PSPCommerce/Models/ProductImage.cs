namespace PSPCommerce.Models
{
    public class ProductImage : BaseModel
    {
        public int ProductID { get; set; }
        public Product _Product { get; set; } = null!;

        public string ImageUrl { get; set; }
    }
}

