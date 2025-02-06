using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.RegisterCustomer
{
    public class RegisterCustomerCommand : IRequest<Result<string>>
    {
        public RegisterCustomerDto RegisterCustomerDto { get; set; }
    }
}
