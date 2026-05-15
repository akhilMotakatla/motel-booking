using MotelBooking.Application.DTOs.Auth;
using MotelBooking.Application.Interfaces;
using MotelBooking.Domain.Common;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Interfaces;

namespace MotelBooking.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(dto.Email))
            return Result<AuthResponseDto>.Failure("An account with this email already exists.");

        if (dto.Password != dto.ConfirmPassword)
            return Result<AuthResponseDto>.Failure("Passwords do not match.");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email.ToLowerInvariant(),
            PasswordHash = _passwordService.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = "Customer"
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<AuthResponseDto>.Success(BuildAuthResponse(user));
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email.ToLowerInvariant());
        if (user == null || !_passwordService.VerifyPassword(dto.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure("Invalid email or password.");

        return Result<AuthResponseDto>.Success(await PersistRefreshTokenAsync(user));
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
        if (principal == null)
            return Result<AuthResponseDto>.Failure("Invalid access token.");

        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(dto.RefreshToken);
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<AuthResponseDto>.Failure("Refresh token is invalid or expired.");

        return Result<AuthResponseDto>.Success(await PersistRefreshTokenAsync(user));
    }

    public async Task<Result> LogoutAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return Result.Failure("User not found.");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    private async Task<AuthResponseDto> PersistRefreshTokenAsync(User user)
    {
        user.RefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return BuildAuthResponse(user);
    }

    private AuthResponseDto BuildAuthResponse(User user) => new()
    {
        UserId = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role,
        AccessToken = _tokenService.GenerateAccessToken(user),
        RefreshToken = user.RefreshToken ?? string.Empty,
        AccessTokenExpiry = DateTime.UtcNow.AddMinutes(60),
        ProfileImageUrl = user.ProfileImageUrl
    };
}
