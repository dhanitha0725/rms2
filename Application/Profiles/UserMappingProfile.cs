using Application.DTOs.UserDtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<RegisterCustomerDto, UserDto>()
                .ForMember(
                    dest => dest.Roles, opt =>
                    opt.MapFrom(src => new List<string> { "Customer" }));

            // map RegisterCustomerDto to User
            CreateMap<RegisterCustomerDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Customer"))
                .ForMember(dest => dest.Payments, opt => opt.Ignore());
           

            // map AddUserDto to User
            CreateMap<AddUserDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore());

            // map User to UserDetailsDto
            CreateMap<User, UserDetailsDto>();

            CreateMap<User, CustomerDetailsDto>();
        }

    }
}
