using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto> UpdateProfileAsync(int userId, UpdateUserDto dto);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    string GenerateJwtToken(User user);
    Task<UserDto> GetUserByIdAsync(string id);
}