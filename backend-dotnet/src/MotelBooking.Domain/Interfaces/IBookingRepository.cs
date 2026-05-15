using MotelBooking.Domain.Common;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Enums;

namespace MotelBooking.Domain.Interfaces;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<Booking?> GetWithDetailsAsync(int bookingId);
    Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
    Task<PagedResult<Booking>> GetAllPagedAsync(int page, int pageSize, BookingStatus? status = null);
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<Booking>> GetUpcomingCheckInsAsync(int days = 7);
    Task<decimal> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null);
    Task<int> GetBookingCountByStatusAsync(BookingStatus status);
    Task<Booking?> GetByConfirmationNumberAsync(string confirmationNumber);
}
