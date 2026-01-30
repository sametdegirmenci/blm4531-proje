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
    private readonly ILoginLogRepository _loginLogRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository, 
        ILoginLogRepository loginLogRepository,
        JwtTokenService jwtTokenService, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _loginLogRepository = loginLogRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            throw new ConflictException("A user with this email already exists.");
        }

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
        var token = _jwtTokenService.GenerateToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto(token, userDto);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null)
        {
            await _loginLogRepository.AddAsync(new LoginLog { Email = dto.Email, AttemptDate = DateTime.UtcNow, IsSuccess = false, FailureReason = "User not found" });
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            await _loginLogRepository.AddAsync(new LoginLog { Email = dto.Email, UserId = user.UserId, AttemptDate = DateTime.UtcNow, IsSuccess = false, FailureReason = "Invalid password" });
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
             await _loginLogRepository.AddAsync(new LoginLog { Email = dto.Email, UserId = user.UserId, AttemptDate = DateTime.UtcNow, IsSuccess = false, FailureReason = "Account deactivated" });
            throw new UnauthorizedException("This account has been deactivated.");
        }

        await _loginLogRepository.AddAsync(new LoginLog { Email = dto.Email, UserId = user.UserId, AttemptDate = DateTime.UtcNow, IsSuccess = true });
        
        var token = _jwtTokenService.GenerateToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto(token, userDto);
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        if (user.Email.ToLower() != dto.Email.ToLower() && await _userRepository.EmailExistsAsync(dto.Email))
        {
            throw new ConflictException("This email is already in use by another account.");
        }

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        
        await _userRepository.UpdateAsync(user);

        return _mapper.Map<UserDto>(user);
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
            throw new NotFoundException(nameof(User), userId);
        }
        return _mapper.Map<UserDto>(user);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedException("Incorrect current password.");
        }

        // Hash new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        
        await _userRepository.UpdateAsync(user);
    }
}