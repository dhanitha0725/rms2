using Application.Abstractions.Interfaces;
using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Features.AuthenticateUser.LogAdmin
{
    public class AdminLoginCommandHandler(
        IAuthService authService,
        ILogger logger) 
        : IRequestHandler<AdminLoginCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            AdminLoginCommand request, 
            CancellationToken cancellationToken)
        {
            var result = await authService.LoginAsync(
                request.LoginDto.Email, 
                request.LoginDto.Password);

            return !result.IsSuccess ?
                Result<string>.Failure(new Error("Can not login now. Try again later.")) 
                : Result<string>.Success(result.Value);
        }
    }
}
