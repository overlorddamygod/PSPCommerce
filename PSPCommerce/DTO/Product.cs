using System.ComponentModel.DataAnnotations;
using PSPCommerce.DTO;

public class ProductSearchParamsDto
{   
    public string? Q { get; set; } = String.Empty;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    [Range(1, int.MaxValue)]
    public int PageSize { get; set; } = 15;
    public int? Category { get; set; } = null;

    public int minPrice { get; set; } = 0;
    public int maxPrice { get; set; } = 100000;
}
