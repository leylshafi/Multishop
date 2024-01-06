using Multishop.Models.Base;

namespace Multishop.Models
{
	public class Setting:BaseEntity
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}
}
