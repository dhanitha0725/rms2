using Application.Abstractions.Interfaces;
using Application.DTOs.UserDtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageUsers.GetUserDetails
{
    public class GetUserDetailsQueryHandler(
        IGenericRepository<User, int> userRepository)
        : IRequestHandler<GetUserDetailsQuery, List<UserDetailsDto>>
    {
        public async Task<List<UserDetailsDto>> Handle(
            GetUserDetailsQuery request, 
            CancellationToken cancellationToken)
        {
            var users = await userRepository.GetAllAsync(cancellationToken);
            return users.Select(u => new UserDetailsDto
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role
            }).ToList();
        }
    }
}
