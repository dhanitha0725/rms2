using Application.DTOs.UserDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageUsers.AddUser
{
    public class AddUserCommand : IRequest<Result<AddUserResponseDto>>
    {
        public AddUserDto AddUserDto { get; set; }
    }
}
