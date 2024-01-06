using System.ComponentModel.DataAnnotations;

namespace Multishop.Areas.Admin.ViewModels
{
    public class UpdateSlideVM
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? Photo { get; set; }
        public string? ImageUrl { get; set; }
        public string? ButtonTitle { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}
