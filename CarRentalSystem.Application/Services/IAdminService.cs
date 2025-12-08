using CarRentalSystem.Application.DTOs;

namespace CarRentalSystem.Application.Services;

public interface IAdminService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int id);
    Task UpdateUserRoleAsync(int id, string newRole);
    Task DeleteUserAsync(int id);
}
