using Application.DTOs.UserDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageUsers.GetCustomerDetails
{
    public class GetCustomerDetailsQuery : IRequest<Result<List<CustomerDetailsDto>>>
    {
    }
}
