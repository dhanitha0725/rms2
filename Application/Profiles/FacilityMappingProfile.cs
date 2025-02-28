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

            CreateMap<FacilityType, GetFacilityTypeDto>();

            CreateMap<RoomConfigurationDto, Room>()
                .ForMember(dest => dest.RoomID, opt => opt.Ignore())
                .ForMember(dest => dest.FacilityID, opt => opt.Ignore())
                .ForMember(dest => dest.RoomNumber, opt => opt.Ignore());
        }
    }
}
