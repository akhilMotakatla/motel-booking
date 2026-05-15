using Microsoft.EntityFrameworkCore;
using MotelBooking.Domain.Common;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Enums;
using MotelBooking.Domain.Interfaces;
using MotelBooking.Infrastructure.Data;

namespace MotelBooking.Infrastructure.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Booking?> GetWithDetailsAsync(int bookingId) =>
        await _dbSet
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r.RoomType)
            .Include(b => b.Room).ThenInclude(r => r.Images.Where(i => i.IsPrimary))
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId) =>
        await _dbSet
            .Include(b => b.Room).ThenInclude(r => r.RoomType)
            .Include(b => b.Room).ThenInclude(r => r.Images.Where(i => i.IsPrimary))
            .Include(b => b.Payment)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CheckInDate)
            .ToListAsync();

    public async Task<PagedResult<Booking>> GetAllPagedAsync(int page, int pageSize, BookingStatus? status = null)
    {
        var query = _dbSet
            .Include(b => b.User)
            .Include(b => b.Room)
            .Include(b => b.Payment)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Booking>
        {
            Items = items,
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime start, DateTime end) =>
        await _dbSet.Where(b => b.CheckInDate >= start && b.CheckOutDate <= end).ToListAsync();

    public async Task<IEnumerable<Booking>> GetUpcomingCheckInsAsync(int days = 7)
    {
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(days);
        return await _dbSet
            .Include(b => b.User)
            .Include(b => b.Room)
            .Where(b => b.CheckInDate >= from && b.CheckInDate <= to
                && b.Status == BookingStatus.Confirmed)
            .OrderBy(b => b.CheckInDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null)
    {
        var query = _dbSet.Where(b => b.Status != BookingStatus.Cancelled);
        if (from.HasValue) query = query.Where(b => b.CreatedAt >= from.Value);
        if (to.HasValue) query = query.Where(b => b.CreatedAt <= to.Value);
        return await query.SumAsync(b => b.TotalAmount);
    }

    public async Task<int> GetBookingCountByStatusAsync(BookingStatus status) =>
        await _dbSet.CountAsync(b => b.Status == status);

    public async Task<Booking?> GetByConfirmationNumberAsync(string confirmationNumber) =>
        await _dbSet
            .Include(b => b.User)
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.ConfirmationNumber == confirmationNumber);
}
