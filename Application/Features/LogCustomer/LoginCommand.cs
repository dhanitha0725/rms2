using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.LogCustomer
{
    public class LoginCommand : IRequest<Result<string>>
    {
        public LoginDto LoginDto { get; set; }
    }
}
