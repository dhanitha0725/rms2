using Application.DTOs.UserDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.AuthenticateUser.LogCustomer
{
    public class LoginCommand : IRequest<Result<string>>
    {
        public LoginDto LoginDto { get; set; }
    }
}
