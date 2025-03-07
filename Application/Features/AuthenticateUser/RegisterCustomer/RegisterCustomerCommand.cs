using Application.DTOs.UserDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.AuthenticateUser.RegisterCustomer
{
    public class RegisterCustomerCommand : IRequest<Result<string>>
    {
        public RegisterCustomerDto RegisterCustomerDto { get; set; }
    }
}
