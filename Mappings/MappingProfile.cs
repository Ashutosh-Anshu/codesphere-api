using AutoMapper;
using codesphere_api.DTOs;
using codesphere_api.Models;

namespace codesphere_api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(
                   dest => dest.ProductId,
                    opt => opt.MapFrom(src => src.Id)
                )
                .ReverseMap();
        }

    }
}
