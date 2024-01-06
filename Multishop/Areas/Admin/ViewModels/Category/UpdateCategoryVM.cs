namespace Multishop.Areas.Admin.ViewModels
{
	public class UpdateCategoryVM
	{
		public string Name { get; set; }
		public string? ImageUrl { get; set; }
		public IFormFile? Photo { get; set; }
	}
}
