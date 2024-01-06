using System.ComponentModel.DataAnnotations;

namespace Multishop.Areas.Admin.ViewModels;

public class CreateSlideVM
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(50, ErrorMessage = "Max length is 50")]
    public string Title { get; set; }
    public string? Description { get; set; }
    public IFormFile Photo { get; set; }
    public string? ButtonTitle { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
}
