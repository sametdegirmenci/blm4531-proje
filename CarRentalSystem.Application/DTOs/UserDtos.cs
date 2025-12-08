namespace CarRentalSystem.Application.DTOs;

public record RegisterUserDto(string FullName, string Email, string Password);

public record LoginDto(string Email, string Password);

public record UserDto(int UserId, string FullName, string Email, string Role, DateTime CreatedAt, bool IsActive);

public record UpdateUserDto(string FullName, string Email);
