using AutoMapper;
using Multishop.Areas.Admin.ViewModels;
using Multishop.Models;

namespace Multishop.Areas.Admin.MappingProfiles
{
    public class SlideProfile : Profile
    {
        public SlideProfile()
        {
            CreateMap<CreateSlideVM, Slide>();
            CreateMap<UpdateSlideVM, Slide>().ReverseMap();
        }
    }
}
