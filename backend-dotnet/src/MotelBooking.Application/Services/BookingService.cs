using MotelBooking.Application.DTOs.Booking;
using MotelBooking.Application.Interfaces;
using MotelBooking.Domain.Common;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Enums;
using MotelBooking.Domain.Interfaces;

namespace MotelBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private const decimal TaxRate = 0.12m;

    public BookingService(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result<BookingDto>> CreateBookingAsync(int userId, CreateBookingDto dto)
    {
        if (dto.CheckInDate >= dto.CheckOutDate)
            return Result<BookingDto>.Failure("Check-out date must be after check-in date.");

        if (dto.CheckInDate.Date < DateTime.UtcNow.Date)
            return Result<BookingDto>.Failure("Check-in date cannot be in the past.");

        var room = await _unitOfWork.Rooms.GetWithDetailsAsync(dto.RoomId);
        if (room == null) return Result<BookingDto>.Failure("Room not found.");

        if (room.Status != RoomStatus.Available)
            return Result<BookingDto>.Failure("Room is not available for booking.");

        var available = await _unitOfWork.Rooms.IsRoomAvailableAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
        if (!available) return Result<BookingDto>.Failure("Room is not available for selected dates.");

        if (dto.NumberOfGuests > room.MaxOccupancy)
            return Result<BookingDto>.Failure($"Room max occupancy is {room.MaxOccupancy} guests.");

        var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
        var subtotal = room.PricePerNight * nights;
        var tax = Math.Round(subtotal * TaxRate, 2);

        var booking = new Booking
        {
            UserId = userId,
            RoomId = dto.RoomId,
            LocationId = dto.LocationId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            NumberOfGuests = dto.NumberOfGuests,
            TotalAmount = subtotal + tax,
            TaxAmount = tax,
            Status = BookingStatus.Confirmed,
            SpecialRequests = dto.SpecialRequests,
            ConfirmationNumber = GenerateConfirmationNumber()
        };

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Bookings.GetWithDetailsAsync(booking.Id);

        // Send confirmation email — fire-and-forget so it never blocks the response
        _ = Task.Run(async () =>
        {
            try { await _emailService.SendBookingConfirmationAsync(created!); }
            catch { /* email failure must not fail the booking */ }
        });

        return Result<BookingDto>.Success(MapBooking(created!));
    }

    public async Task<Result<BookingDto>> GetBookingByIdAsync(int bookingId, int userId, string role)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsAsync(bookingId);
        if (booking == null) return Result<BookingDto>.Failure("Booking not found.");

        if (role != "Admin" && booking.UserId != userId)
            return Result<BookingDto>.Failure("Access denied.");

        return Result<BookingDto>.Success(MapBooking(booking));
    }

    public async Task<Result<IEnumerable<BookingDto>>> GetUserBookingsAsync(int userId)
    {
        var bookings = await _unitOfWork.Bookings.GetByUserIdAsync(userId);
        return Result<IEnumerable<BookingDto>>.Success(bookings.Select(MapBooking));
    }

    public async Task<Result<PagedResult<BookingDto>>> GetAllBookingsAsync(int page, int pageSize, BookingStatus? status)
    {
        var paged = await _unitOfWork.Bookings.GetAllPagedAsync(page, pageSize, status);
        var result = new PagedResult<BookingDto>
        {
            Items = paged.Items.Select(MapBooking),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
        return Result<PagedResult<BookingDto>>.Success(result);
    }

    public async Task<Result<BookingDto>> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsAsync(bookingId);
        if (booking == null) return Result<BookingDto>.Failure("Booking not found.");

        booking.Status = status;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();

        return Result<BookingDto>.Success(MapBooking(booking));
    }

    public async Task<Result> CancelBookingAsync(int bookingId, int userId, string role)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) return Result.Failure("Booking not found.");

        if (role != "Admin" && booking.UserId != userId)
            return Result.Failure("Access denied.");

        if (booking.Status == BookingStatus.CheckedIn || booking.Status == BookingStatus.CheckedOut)
            return Result.Failure("Cannot cancel a booking that is already checked in or completed.");

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    private static string GenerateConfirmationNumber() =>
        $"MB{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";

    private static BookingDto MapBooking(Booking b) => new()
    {
        Id = b.Id,
        UserId = b.UserId,
        UserName = b.User?.FullName ?? string.Empty,
        UserEmail = b.User?.Email ?? string.Empty,
        RoomId = b.RoomId,
        RoomName = b.Room?.Name ?? string.Empty,
        RoomNumber = b.Room?.RoomNumber ?? string.Empty,
        RoomThumbnail = b.Room?.ThumbnailUrl,
        CheckInDate = b.CheckInDate,
        CheckOutDate = b.CheckOutDate,
        NightsCount = b.NightsCount,
        NumberOfGuests = b.NumberOfGuests,
        TotalAmount = b.TotalAmount,
        TaxAmount = b.TaxAmount,
        DiscountAmount = b.DiscountAmount,
        Status = b.Status.ToString(),
        SpecialRequests = b.SpecialRequests,
        ConfirmationNumber = b.ConfirmationNumber,
        CreatedAt = b.CreatedAt,
        PaymentStatus = b.Payment?.Status.ToString()
    };
}
