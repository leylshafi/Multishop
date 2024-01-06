using Multishop.Models.Base;

namespace Multishop.Models
{
    public class Size:BaseNameableEntity
    {
        public List<ProductSize>? ProductSizes { get; set; }
    }
}
