using Application.DTOs.UserDtos;
using MediatR;

namespace Application.Features.ManageUsers.GetUserDetails
{
    public record GetUserDetailsQuery : IRequest<List<UserDetailsDto>>
    {
    }
}
