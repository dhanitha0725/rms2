using System.Text.Json;
using Application.DTOs.FacilityDtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class FacilityMappingProfile : Profile
    {
        public FacilityMappingProfile()
        {
            CreateMap<AddFacilityDto, Facility>()
                .ForMember(dest => dest.Attributes, opt => opt.Ignore())
                .ForMember(dest => dest.FacilityTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<AddFacilityTypeDto, FacilityType>();

            // Map FacilityType to FacilityTypeDto for automatic projection.
            CreateMap<FacilityType, GetFacilityTypeDto>();
        }
    }
}
