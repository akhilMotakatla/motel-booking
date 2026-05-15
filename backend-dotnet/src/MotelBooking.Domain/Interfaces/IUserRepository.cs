using MotelBooking.Domain.Entities;

namespace MotelBooking.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetWithBookingsAsync(int userId);
}
