using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.AuthenticateUser.RegisterCustomer
{
    public class RegisterCustomerCommandHandler (
        IAuthService authService,
        ILogger logger,
        IGenericRepository<User, int> userRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork) :
        IRequestHandler<RegisterCustomerCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            RegisterCustomerCommand request,
            CancellationToken cancellationToken)
        {
            // this handler only for customer registrations
            // automatically assign customer role to the user
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                //check user entity user exists
                var userExists = await userRepository.ExistsAsync(u =>
                    u.Email == request.RegisterCustomerDto.Email, cancellationToken);

                if (userExists)
                {
                    logger.Warning($"User with email {request.RegisterCustomerDto.Email} already exists.");
                    return Result<string>.Failure(new Error("User already exists."));
                }

                // Create identity user
                var identityResult = await authService.CreateUserAsync(
                    request.RegisterCustomerDto.Email,
                    request.RegisterCustomerDto.Password,
                    request.RegisterCustomerDto.ConfirmPassword);
                if (!identityResult.IsSuccess)
                {
                    logger.Error("Failed to create identity user: {error}", identityResult.Error);
                    return (Result<string>)identityResult;
                }

                var user = mapper.Map<User>(request.RegisterCustomerDto);

                await userRepository.AddAsync(user, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information($"User {user.UserId} registered successfully");
                return (Result<string>)identityResult;
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error registering user");
                throw;
            }


        }
    }
}
