using Application.DTOs.UserDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.AuthenticateUser.LogAdmin
{
    public class AdminLoginCommand : IRequest<Result<string>>
    {
        public LoginDto LoginDto { get; set; }
    }
}
