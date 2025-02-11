using System.Transactions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.RegisterCustomer
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
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

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
                    logger.Error("Failed to create identity user");
                }

            // Create user entity using AutoMapper
            var user = mapper.Map<User>(request.RegisterCustomerDto);
        
            // Save the user entity to the repository
            await userRepository.AddAsync(user, cancellationToken);

            // Commit the changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            transaction.Complete();

            logger.Information($"User {user.UserId} registered successfully");
            return (Result<string>)identityResult;
        }
    }
}
