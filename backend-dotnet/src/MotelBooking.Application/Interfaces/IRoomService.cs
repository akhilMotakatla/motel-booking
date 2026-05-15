using MotelBooking.Application.DTOs.Room;
using MotelBooking.Domain.Common;

namespace MotelBooking.Application.Interfaces;

public interface IRoomService
{
    Task<Result<PagedResult<RoomDto>>> GetRoomsAsync(RoomFilterDto filter);
    Task<Result<RoomDto>> GetRoomByIdAsync(int id);
    Task<Result<IEnumerable<RoomDto>>> GetFeaturedRoomsAsync();
    Task<Result<IEnumerable<RoomDto>>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests);
    Task<Result<RoomDto>> CreateRoomAsync(CreateRoomDto dto);
    Task<Result<RoomDto>> UpdateRoomAsync(int id, UpdateRoomDto dto);
    Task<Result> DeleteRoomAsync(int id);
    Task<Result<bool>> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut);
}
