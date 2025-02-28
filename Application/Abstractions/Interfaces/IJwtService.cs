using Application.DTOs.UserDtos;

namespace Application.Abstractions.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(UserDto user);
    }
}
