namespace CarRentalSystem.Application.DTOs;

public record AuthResponseDto(string Token, UserDto User);

public record ChangePasswordDto(string CurrentPassword, string NewPassword);
