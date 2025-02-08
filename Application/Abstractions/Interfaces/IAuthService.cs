using Domain.Common;

namespace Application.Abstractions.Interfaces
{
    public interface IAuthService
    {
        Task<Result> CreateUserAsync(string email, string password, string confirmPassword);
        Task<Result<string>> LoginAsync(string email, string password);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<Result<string>> AddUserAsync(string email, string password, string role);
    }
}
