using AutoMapper;
using Multishop.Areas.Admin.ViewModels;
using Multishop.Models;

namespace Multishop.Areas.Admin.MappingProfiles
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductVM, Product>().ReverseMap();
        }
    }
}
