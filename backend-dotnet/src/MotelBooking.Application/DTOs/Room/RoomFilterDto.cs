namespace MotelBooking.Application.DTOs.Room;

public class RoomFilterDto
{
    public string? Search { get; set; }
    public int? RoomTypeId { get; set; }
    public int? LocationId { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MaxOccupancy { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public string? SortBy { get; set; } = "Price";
    public bool SortDescending { get; set; } = false;
}
