using Multishop.Models.Base;

namespace Multishop.Models;

public class Category:BaseNameableEntity
{
    public string ImageUrl { get; set; } = null!;
    public List<Product>? Products { get; set; }
}
