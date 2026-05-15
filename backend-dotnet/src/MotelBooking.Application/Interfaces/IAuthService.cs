using MotelBooking.Application.DTOs.Auth;
using MotelBooking.Domain.Common;

namespace MotelBooking.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto);
    Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
    Task<Result> LogoutAsync(int userId);
}
