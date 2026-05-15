using MotelBooking.Domain.Common;
using MotelBooking.Domain.Enums;

namespace MotelBooking.Domain.Entities;

public class Booking : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; } = 1;
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? SpecialRequests { get; set; }
    public string? ConfirmationNumber { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public Payment? Payment { get; set; }

    public int NightsCount => (CheckOutDate - CheckInDate).Days;
}
