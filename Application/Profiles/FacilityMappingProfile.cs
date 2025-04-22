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
                .ForMember(dest => dest.Images,
                    opt => opt.Ignore());

            CreateMap<AddFacilityTypeDto, FacilityType>();

            CreateMap<FacilityType, GetFacilityTypeDto>();

            CreateMap<RoomConfigurationDto, Room>()
                .ForMember(dest => dest.RoomID,
                    opt => opt.Ignore())
                .ForMember(dest => dest.FacilityID,
                    opt => opt.Ignore());

            // Map Facility to FullFacilityDetailsDto
            CreateMap<Facility, FullFacilityDetailsDto>()
                .ForMember(dest => dest.FacilityID, opt => opt.MapFrom(src => src.FacilityID))
                .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => src.FacilityType.TypeName))
                .ForMember(dest => dest.ChildFacilities, opt => opt.MapFrom(src =>
                    src.ChildFacilities ?? new List<Facility>()))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images.Select(i => i.ImageUrl).ToList()))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src =>
                    src.Attributes ?? new List<string>()));
        }
    }
}

