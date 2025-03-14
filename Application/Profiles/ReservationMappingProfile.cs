using Application.DTOs.ReservationDtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class ReservationMappingProfile : Profile
    {
        public ReservationMappingProfile()
        {
            CreateMap<SelectedPackageDto, ReservedPackage>();
        }
    }
}
