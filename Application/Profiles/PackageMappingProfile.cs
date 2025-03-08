using Application.DTOs.PackageDto;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class PackageMappingProfile : Profile
    {
        public PackageMappingProfile()
        {
            CreateMap<AddPackageDto, Package>()
                .ForMember(dest => dest.Pricings, opt => opt.MapFrom(src => src.Pricings.Select(x => new Pricing
                {
                    Sector = x.Key,
                    Price = x.Value
                }).ToList()));
        }
    }
}
