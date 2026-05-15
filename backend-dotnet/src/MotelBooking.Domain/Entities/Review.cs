using MotelBooking.Domain.Common;

namespace MotelBooking.Domain.Entities;

public class Review : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;
    public bool IsApproved { get; set; } = false;
    public int CleanlinessRating { get; set; }
    public int ServiceRating { get; set; }
    public int ValueRating { get; set; }
    public int LocationRating { get; set; }
}
