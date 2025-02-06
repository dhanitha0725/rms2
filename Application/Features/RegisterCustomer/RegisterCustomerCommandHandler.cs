using System.Transactions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.RegisterCustomer
{
    public class RegisterCustomerCommandHandler :
                    IRequestHandler<RegisterCustomerCommand,
                        Result<string>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger _logger;
        private readonly IGenericRepository<User, int> _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCustomerCommandHandler(
            IAuthService authService,
            ILogger logger,
            IGenericRepository<User, int> userRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork
             )
        {
            _authService = authService;
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(
            RegisterCustomerCommand request,
            CancellationToken cancellationToken)
        {
            using var transaction = new TransactionScope(
                TransactionScopeAsyncFlowOption.Enabled);

                // Create identity user
                var identityResult = await _authService.CreateUserAsync(
                    request.RegisterCustomerDto.Email,
                    request.RegisterCustomerDto.Password,
                    request.RegisterCustomerDto.ConfirmPassword);
                if (!identityResult.IsSuccess)
                {
                    _logger.Error("Failed to create identity user");
                }

            // Create user entity using AutoMapper
            var user = _mapper.Map<User>(request.RegisterCustomerDto);
        
            // Save the user entity to the repository
            await _userRepository.AddAsync(user, cancellationToken);

            // Commit the changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            transaction.Complete();

            _logger.Information($"User {user.UserId} registered successfully");
            return (Result<string>)identityResult;
        }
    }
}
