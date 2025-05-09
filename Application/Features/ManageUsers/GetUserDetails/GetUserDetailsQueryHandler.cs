using Application.Abstractions.Interfaces;
using Application.DTOs.UserDtos;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageUsers.GetUserDetails
{
    public class GetUserDetailsQueryHandler(
            IGenericRepository<User, int> userRepository,
            IMapper mapper)
            : IRequestHandler<GetUserDetailsQuery, List<UserDetailsDto>>
    {
        public async Task<List<UserDetailsDto>> Handle(
            GetUserDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var users = await userRepository.GetAllAsync(cancellationToken);

            var filteredUsers = users.Where(u =>
                !u.Role.Equals("customer", StringComparison.OrdinalIgnoreCase) &&
                !u.Role.Equals("guest", StringComparison.OrdinalIgnoreCase));

            return mapper.Map<List<UserDetailsDto>>(filteredUsers);
        }
    }
}
