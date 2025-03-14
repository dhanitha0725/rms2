using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CreateGuestUser
{
    public class CreateGuestUserCommandHandler (
        IGenericRepository<User, int> userRepository,
        IUnitOfWork unitOfWork,
        ILogger logger) 
        : IRequestHandler<CreateGuestUserCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            CreateGuestUserCommand request, 
            CancellationToken cancellationToken)
        {
            try
            {
                // Check if the user already exists
                var guestExists = await userRepository.ExistsAsync(
                    x => x.Email == request.GuestEmail, cancellationToken);

                if (guestExists)
                {
                    var existingGuest = (await userRepository.GetAllAsync(cancellationToken))
                        .First(x => x.Email == request.GuestEmail);
                    return Result<int>.Success(existingGuest.UserId);
                }

                var guestUser = new User
                {
                    Email = request.GuestEmail,
                    PhoneNumber = request.GuestPhone,
                    FirstName = request.GuestFirstName,
                    LastName = request.GuestLastName,
                    Role = "Guest"
                };

                await userRepository.AddAsync(guestUser, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
              
                return Result<int>.Success(guestUser.UserId);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error creating guest user");
                throw;  
            }
        }
    }
}
