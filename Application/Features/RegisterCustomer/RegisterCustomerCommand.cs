using Application.DTOs.UserDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.RegisterCustomer
{
    public class RegisterCustomerCommand : IRequest<Result<string>>
    {
        public RegisterCustomerDto RegisterCustomerDto { get; set; }
    }
}
