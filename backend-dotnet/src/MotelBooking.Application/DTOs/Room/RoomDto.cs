namespace MotelBooking.Application.DTOs.Room;

public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxOccupancy { get; set; }
    public int FloorNumber { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public double SizeInSqFt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsFeatured { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<AmenityDto> Amenities { get; set; } = new();
}

public class AmenityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Category { get; set; }
}
