using MotelBooking.Domain.Common;

namespace MotelBooking.Domain.Entities;

public class RoomType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
