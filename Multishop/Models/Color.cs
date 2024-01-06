using Multishop.Models.Base;

namespace Multishop.Models
{
    public class Color:BaseNameableEntity
    {
        public List<ProductColor>? ProductColors { get; set; }
    }
}
