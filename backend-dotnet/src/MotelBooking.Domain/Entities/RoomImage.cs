using MotelBooking.Domain.Common;

namespace MotelBooking.Domain.Entities;

public class RoomImage : BaseEntity
{
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; } = false;
    public int SortOrder { get; set; } = 0;
}
