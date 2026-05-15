using System.Security.Claims;
using MotelBooking.Domain.Entities;

namespace MotelBooking.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
