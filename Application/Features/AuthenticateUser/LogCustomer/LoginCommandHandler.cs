using Application.Abstractions.Interfaces;
using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Features.AuthenticateUser.LogCustomer
{
    public class LoginCommandHandler(
        IAuthService authService,
        ILogger logger) : IRequestHandler<LoginCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            LoginCommand request, 
            CancellationToken cancellationToken)
        {
            var result = await authService.LoginAsync(
                request.LoginDto.Email,
                request.LoginDto.Password);

            if (!result.IsSuccess)
            {
                logger.Warning($"Failed login attempt for {request.LoginDto.Email}");
            }

            return result;
        }
    }
}
