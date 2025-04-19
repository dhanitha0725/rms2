using Application.Abstractions.Interfaces;
using Application.DTOs.UserDtos;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageUsers.GetCustomerDetails
{
    public class GetCustomerDetailsQueryHandler(
            IGenericRepository<User, int> userRepository,
            IMapper mapper) :
            IRequestHandler<GetCustomerDetailsQuery, Result<List<CustomerDetailsDto>>>
    {
        public async Task<Result<List<CustomerDetailsDto>>> Handle(
            GetCustomerDetailsQuery request,
            CancellationToken cancellationToken)
        {
            // Get the customer details from the database
            var customers = await userRepository.GetAllAsync(cancellationToken);

            // users with role "customer" or "guest"
            var filteredCustomers = customers
                .Where(u => u.Role.Equals("customer", StringComparison.OrdinalIgnoreCase) ||
                            u.Role.Equals("guest", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredCustomers != null && !filteredCustomers.Any())
            {
                return Result<List<CustomerDetailsDto>>.Failure(new Error("No customers or guests found."));
            }

            var customerDetails = mapper.Map<List<CustomerDetailsDto>>(filteredCustomers);

            return Result<List<CustomerDetailsDto>>.Success(customerDetails);
        }
    }
}
