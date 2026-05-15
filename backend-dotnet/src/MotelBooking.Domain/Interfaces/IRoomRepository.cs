using MotelBooking.Domain.Common;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Enums;

namespace MotelBooking.Domain.Interfaces;

public interface IRoomRepository : IGenericRepository<Room>
{
    Task<Room?> GetWithDetailsAsync(int roomId);
    Task<PagedResult<Room>> GetPagedAsync(int page, int pageSize, string? search = null,
        int? roomTypeId = null, decimal? minPrice = null, decimal? maxPrice = null,
        int? maxOccupancy = null, RoomStatus? status = null, int? locationId = null,
        string? sortBy = null, bool sortDescending = false);
    Task<IEnumerable<Room>> GetFeaturedRoomsAsync(int count = 6);
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests = 1);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
    Task<IEnumerable<Room>> GetByRoomTypeAsync(int roomTypeId);
}
