using MediatR;
using Application.Abstractions.Interfaces;
using Application.DTOs.UserDtos;
using AutoMapper;
using Domain.Common;
using Serilog;
using Domain.Entities;

namespace Application.Features.ManageUsers.AddUser
{
    public class AddUserCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger,
        IGenericRepository<User, int> userRepository,
        IAuthService authService) : IRequestHandler<AddUserCommand, Result<AddUserResponseDto>>
    {

        public async Task<Result<AddUserResponseDto>>  Handle(
            AddUserCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var userExists = await userRepository.ExistsAsync(u =>
                    u.Email == request.AddUserDto.Email, cancellationToken);

                if (userExists)
                {
                    logger.Warning("User with email {Email} already exists.", request.AddUserDto.Email);
                    return Result<AddUserResponseDto>.Failure(new Error("User already exists."));
                }

                var identityResult = await authService.AddUserAsync(
                    request.AddUserDto.Email,
                    "123456@Rs",
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

                logger.Information("User {UserUserId} added successfully", user.UserId);

                var responseDto = new AddUserResponseDto
                {
                    UserId = user.UserId
                };

                return Result<AddUserResponseDto>.Success(responseDto);
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
