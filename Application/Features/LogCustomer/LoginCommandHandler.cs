using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Features.LogCustomer
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public LoginCommandHandler(
            IAuthService authService,
            ILogger logger,
            IMapper mapper)
        {
            _authService = authService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<string>> Handle(
            LoginCommand request, 
            CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(
                request.LoginDto.Email,
                request.LoginDto.Password);

            if (!result.IsSuccess)
            {
                _logger.Warning($"Failed login attempt for {request.LoginDto.Email}");
            }

            return result;
        }
    }
}
