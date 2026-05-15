using Microsoft.EntityFrameworkCore;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Interfaces;
using MotelBooking.Infrastructure.Data;

namespace MotelBooking.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
        await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

    public async Task<bool> EmailExistsAsync(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email);

    public async Task<User?> GetWithBookingsAsync(int userId) =>
        await _dbSet.Include(u => u.Bookings).ThenInclude(b => b.Room)
            .FirstOrDefaultAsync(u => u.Id == userId);
}
