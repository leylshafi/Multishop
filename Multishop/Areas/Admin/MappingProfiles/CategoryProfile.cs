using AutoMapper;
using Multishop.Areas.Admin.ViewModels;
using Multishop.Models;

namespace Multishop.Areas.Admin.MappingProfiles
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryVM, Category>();
            CreateMap<UpdateCategoryVM, Category>().ReverseMap();
        }
    }
}
