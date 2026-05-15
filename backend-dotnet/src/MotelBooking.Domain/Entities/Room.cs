using MotelBooking.Domain.Common;
using MotelBooking.Domain.Enums;

namespace MotelBooking.Domain.Entities;

public class Room : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxOccupancy { get; set; } = 2;
    public int FloorNumber { get; set; } = 1;
    public string RoomNumber { get; set; } = string.Empty;
    public double SizeInSqFt { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    public string? ThumbnailUrl { get; set; }
    public double AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public bool IsFeatured { get; set; } = false;

    public int RoomTypeId { get; set; }
    public RoomType RoomType { get; set; } = null!;

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();
    public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
