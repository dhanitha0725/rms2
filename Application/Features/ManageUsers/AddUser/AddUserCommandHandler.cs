using MediatR;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using Serilog;
using Domain.Entities;
using System.Transactions;

namespace Application.Features.ManageUsers.AddUser
{
    public class AddUserCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger,
        IGenericRepository<User, int> userRepository,
        IAuthService authService) : IRequestHandler<AddUserCommand, Result<string>>
    {
        
        public async Task<Result<string>> Handle(
            AddUserCommand request, 
            CancellationToken cancellationToken)
        {
            //using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var userExists = await userRepository.ExistsAsync(u =>
                    u.Email == request.AddUserDto.Email, cancellationToken);

                if (userExists)
                {
                    logger.Warning($"User with email {request.AddUserDto.Email} already exists.");
                    return Result<string>.Failure(new Error("User already exists."));
                }

                var identityResult = await authService.AddUserAsync(
                    request.AddUserDto.Email,
                    request.AddUserDto.Password,
                    request.AddUserDto.Role);

                if (identityResult.IsSuccess)
                {
                    logger.Error("Failed to create identity user");
                }

                // Create user entity using AutoMapper
                var user = mapper.Map<User>(request.AddUserDto);

                await userRepository.AddAsync(user, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                //transaction.Complete();

                logger.Information($"User {user.UserId} added successfully");
                return Result<string>.Success("User added successfully");
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error adding user");
                throw;
            }
        }
    }
}
