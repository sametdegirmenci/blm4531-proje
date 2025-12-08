using CarRentalSystem.Application.Common;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AraçKontrol_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        var response = new ApiResponse<IEnumerable<UserDto>>(users);
        return Ok(response);
    }

    /// <summary>
    /// Get a specific user by ID (Admin only)
    /// </summary>
    [HttpGet("users/{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _adminService.GetUserByIdAsync(id);
        var response = new ApiResponse<UserDto>(user);
        return Ok(response);
    }
    
    /// <summary>
    /// Update a user's role (Admin only)
    /// </summary>
    [HttpPut("users/{id}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
    {
        await _adminService.UpdateUserRoleAsync(id, dto.NewRole);
        return NoContent();
    }

    /// <summary>
    /// Delete a user (Admin only)
    /// </summary>
    [HttpDelete("users/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _adminService.DeleteUserAsync(id);
        return NoContent();
    }
}

// A DTO for the role update payload
public record UpdateUserRoleDto(string NewRole);
