using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class FacilityMappingProfile : Profile
    {
        public FacilityMappingProfile()
        {
            // map add facility dto to facility
            CreateMap<AddFacilityDto, Facility>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => (string?)null));
        }
    }
}
