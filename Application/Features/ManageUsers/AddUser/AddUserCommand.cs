using Application.DTOs.UserDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageUsers.AddUser
{
    public class AddUserCommand : IRequest<Result<string>>, IBaseRequest
    {
        public AddUserDto AddUserDto { get; set; }
    }
}
