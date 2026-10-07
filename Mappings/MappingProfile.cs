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

            CreateMap<ApplicationRole, RoleDetailDTO>()
                .ForMember(
                   dest => dest.RoleId,
                    opt => opt.MapFrom(src => src.Id)
                )
                .ReverseMap();

            CreateMap<ApplicationUser, UserDetailDTO>()
                .ForMember(
                   dest => dest.UserId,
                    opt => opt.MapFrom(src => src.Id)
                )
                .ReverseMap();

            
        }

    }
}
