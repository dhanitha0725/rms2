using MediatR;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using Serilog;
using Domain.Entities;
using System.Transactions;

namespace Application.Features.AddUser
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IGenericRepository<User, int> _userRepository;
        private readonly IAuthService _authService;

        public AddUserCommandHandler(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger logger, 
            IGenericRepository<User, int> userRepository, 
            IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger; 
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<Result<string>> Handle(
            AddUserCommand request, 
            CancellationToken cancellationToken)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var userExists = await _userRepository.ExistsAsync(u =>
                u.Email == request.AddUserDto.Email, cancellationToken);

            if (userExists)
            {
                _logger.Warning($"User with email {request.AddUserDto.Email} already exists.");
                return Result<string>.Failure(new Error("User already exists."));
            }

            var identityResult = await _authService.AddUserAsync(
                request.AddUserDto.Email, 
                request.AddUserDto.Password, 
                request.AddUserDto.Role);

            if (identityResult.IsSuccess)
            {
                _logger.Error("Failed to create identity user");
            }

            // Create user entity using AutoMapper
            var user = _mapper.Map<User>(request.AddUserDto);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            transaction.Complete();

            _logger.Information($"User {user.UserId} added successfully");
            return Result<string>.Success("User added successfully");
        }
    }
}
