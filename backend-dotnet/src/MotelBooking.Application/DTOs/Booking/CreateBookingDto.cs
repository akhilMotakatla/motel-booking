namespace MotelBooking.Application.DTOs.Booking;

public class CreateBookingDto
{
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; } = 1;
    public string? SpecialRequests { get; set; }
    public int? LocationId { get; set; }
}
