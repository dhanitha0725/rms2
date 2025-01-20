using Application.DTOs;

namespace Application.Abstractions.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(UserDto user);
    }
}
