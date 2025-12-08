using AutoMapper;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Domain.Exceptions;
using CarRentalSystem.Domain.Interfaces;

namespace CarRentalSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public AuthService(IUserRepository userRepository, JwtTokenService jwtTokenService, IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            throw new ConflictException("A user with this email already exists.");
        }

        // Create new user
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        user = await _userRepository.AddAsync(user);

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user);

        // Map to DTO
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto(token, userDto);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedException("This account has been deactivated.");
        }

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user);

        // Map to DTO
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto(token, userDto);
    }

    public string GenerateJwtToken(User user)
    {
        return _jwtTokenService.GenerateToken(user);
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        if (!int.TryParse(id, out var userId))
        {
            throw new ValidationException("Invalid user ID format.");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }
        return _mapper.Map<UserDto>(user);
    }
}
