using AutoMapper;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Domain.Interfaces;
using CarRentalSystem.Domain.Exceptions;

namespace CarRentalSystem.Application.Implementation;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AdminService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }
        return _mapper.Map<UserDto>(user);
    }

    public async Task UpdateUserRoleAsync(int id, string newRole)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }

        if (Enum.TryParse<UserRole>(newRole, true, out var roleEnum))
        {
            user.Role = roleEnum;
            await _userRepository.UpdateAsync(user);
        }
        else
        {
            throw new DomainException($"Invalid role specified: {newRole}");
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        if (!await _userRepository.ExistsAsync(id))
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }
        await _userRepository.DeleteAsync(id);
    }
}
