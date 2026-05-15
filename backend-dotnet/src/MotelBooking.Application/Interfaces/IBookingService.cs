using MotelBooking.Application.DTOs.Booking;
using MotelBooking.Domain.Common;
using MotelBooking.Domain.Enums;

namespace MotelBooking.Application.Interfaces;

public interface IBookingService
{
    Task<Result<BookingDto>> CreateBookingAsync(int userId, CreateBookingDto dto);
    Task<Result<BookingDto>> GetBookingByIdAsync(int bookingId, int userId, string role);
    Task<Result<IEnumerable<BookingDto>>> GetUserBookingsAsync(int userId);
    Task<Result<PagedResult<BookingDto>>> GetAllBookingsAsync(int page, int pageSize, BookingStatus? status);
    Task<Result<BookingDto>> UpdateBookingStatusAsync(int bookingId, BookingStatus status);
    Task<Result> CancelBookingAsync(int bookingId, int userId, string role);
}
