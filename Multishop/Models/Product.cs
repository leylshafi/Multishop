using Multishop.Models.Base;

namespace Multishop.Models;

public class Product:BaseNameableEntity
{
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public string? Description { get; set; }
    public string? FacebookLink { get; set; }
    public string? TwitterLink { get; set; }
    public string? LinkedinLink { get; set; }
    public string? PinterestLink { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<ProductImage>? ProductImages { get; set; }
    public List<ProductSize>? ProductSizes { get; set; }
    public List<ProductColor>? ProductColors { get; set; }
    public Product()
    {
        ProductImages = new();
        ProductColors = new();
        ProductSizes = new();
    }
}
