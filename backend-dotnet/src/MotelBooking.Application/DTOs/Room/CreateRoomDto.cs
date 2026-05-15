namespace MotelBooking.Application.DTOs.Room;

public class CreateRoomDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxOccupancy { get; set; } = 2;
    public int FloorNumber { get; set; } = 1;
    public string RoomNumber { get; set; } = string.Empty;
    public double SizeInSqFt { get; set; }
    public int RoomTypeId { get; set; }
    public bool IsFeatured { get; set; } = false;
    public List<int> AmenityIds { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public string? ThumbnailUrl { get; set; }
}

public class UpdateRoomDto : CreateRoomDto
{
    public int Id { get; set; }
    public string Status { get; set; } = "Available";
}
