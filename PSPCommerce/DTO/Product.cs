using System.ComponentModel.DataAnnotations;
using PSPCommerce.DTO;

public class ProductSearchParamsDto
{   
    public string? Q { get; set; } = String.Empty;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    [Range(1, int.MaxValue)]
    public int PageSize { get; set; } = 15;
}
