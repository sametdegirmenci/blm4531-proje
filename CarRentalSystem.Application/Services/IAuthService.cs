using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto> GetUserByIdAsync(string id);
    string GenerateJwtToken(User user);
}
