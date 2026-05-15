using MotelBooking.Application.DTOs.Room;
using MotelBooking.Application.Interfaces;
using MotelBooking.Domain.Common;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Enums;
using MotelBooking.Domain.Interfaces;

namespace MotelBooking.Application.Services;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<RoomDto>>> GetRoomsAsync(RoomFilterDto filter)
    {
        RoomStatus? status = filter.Status != null && Enum.TryParse<RoomStatus>(filter.Status, out var s) ? s : null;

        var paged = await _unitOfWork.Rooms.GetPagedAsync(
            filter.PageNumber, filter.PageSize,
            filter.Search, filter.RoomTypeId,
            filter.MinPrice, filter.MaxPrice,
            filter.MaxOccupancy, status,
            filter.LocationId, filter.SortBy, filter.SortDescending);

        var result = new PagedResult<RoomDto>
        {
            Items = paged.Items.Select(MapRoom),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Result<PagedResult<RoomDto>>.Success(result);
    }

    public async Task<Result<RoomDto>> GetRoomByIdAsync(int id)
    {
        var room = await _unitOfWork.Rooms.GetWithDetailsAsync(id);
        if (room == null) return Result<RoomDto>.Failure("Room not found.");
        return Result<RoomDto>.Success(MapRoom(room));
    }

    public async Task<Result<IEnumerable<RoomDto>>> GetFeaturedRoomsAsync()
    {
        var rooms = await _unitOfWork.Rooms.GetFeaturedRoomsAsync();
        return Result<IEnumerable<RoomDto>>.Success(rooms.Select(MapRoom));
    }

    public async Task<Result<IEnumerable<RoomDto>>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests)
    {
        var rooms = await _unitOfWork.Rooms.GetAvailableRoomsAsync(checkIn, checkOut, guests);
        return Result<IEnumerable<RoomDto>>.Success(rooms.Select(MapRoom));
    }

    public async Task<Result<RoomDto>> CreateRoomAsync(CreateRoomDto dto)
    {
        var room = new Room
        {
            Name = dto.Name,
            Description = dto.Description,
            PricePerNight = dto.PricePerNight,
            MaxOccupancy = dto.MaxOccupancy,
            FloorNumber = dto.FloorNumber,
            RoomNumber = dto.RoomNumber,
            SizeInSqFt = dto.SizeInSqFt,
            RoomTypeId = dto.RoomTypeId,
            IsFeatured = dto.IsFeatured,
            ThumbnailUrl = dto.ThumbnailUrl,
            Status = RoomStatus.Available
        };

        foreach (var amenityId in dto.AmenityIds)
            room.RoomAmenities.Add(new RoomAmenity { AmenityId = amenityId });

        foreach (var (url, idx) in dto.ImageUrls.Select((u, i) => (u, i)))
            room.Images.Add(new RoomImage { ImageUrl = url, SortOrder = idx, IsPrimary = idx == 0 });

        await _unitOfWork.Rooms.AddAsync(room);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Rooms.GetWithDetailsAsync(room.Id);
        return Result<RoomDto>.Success(MapRoom(created!));
    }

    public async Task<Result<RoomDto>> UpdateRoomAsync(int id, UpdateRoomDto dto)
    {
        var room = await _unitOfWork.Rooms.GetWithDetailsAsync(id);
        if (room == null) return Result<RoomDto>.Failure("Room not found.");

        room.Name = dto.Name;
        room.Description = dto.Description;
        room.PricePerNight = dto.PricePerNight;
        room.MaxOccupancy = dto.MaxOccupancy;
        room.FloorNumber = dto.FloorNumber;
        room.RoomNumber = dto.RoomNumber;
        room.SizeInSqFt = dto.SizeInSqFt;
        room.RoomTypeId = dto.RoomTypeId;
        room.IsFeatured = dto.IsFeatured;
        room.ThumbnailUrl = dto.ThumbnailUrl;
        room.UpdatedAt = DateTime.UtcNow;

        if (Enum.TryParse<RoomStatus>(dto.Status, out var status))
            room.Status = status;

        _unitOfWork.Rooms.Update(room);
        await _unitOfWork.SaveChangesAsync();

        return Result<RoomDto>.Success(MapRoom(room));
    }

    public async Task<Result> DeleteRoomAsync(int id)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null) return Result.Failure("Room not found.");

        room.IsDeleted = true;
        room.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Rooms.Update(room);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<bool>> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var available = await _unitOfWork.Rooms.IsRoomAvailableAsync(roomId, checkIn, checkOut);
        return Result<bool>.Success(available);
    }

    private static RoomDto MapRoom(Room r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Description = r.Description,
        PricePerNight = r.PricePerNight,
        MaxOccupancy = r.MaxOccupancy,
        FloorNumber = r.FloorNumber,
        RoomNumber = r.RoomNumber,
        SizeInSqFt = r.SizeInSqFt,
        Status = r.Status.ToString(),
        ThumbnailUrl = r.ThumbnailUrl,
        AverageRating = r.AverageRating,
        TotalReviews = r.TotalReviews,
        IsFeatured = r.IsFeatured,
        RoomTypeId = r.RoomTypeId,
        RoomTypeName = r.RoomType?.Name ?? string.Empty,
        ImageUrls = r.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).ToList(),
        Amenities = r.RoomAmenities.Select(ra => new AmenityDto
        {
            Id = ra.Amenity?.Id ?? 0,
            Name = ra.Amenity?.Name ?? string.Empty,
            Icon = ra.Amenity?.Icon,
            Category = ra.Amenity?.Category
        }).ToList()
    };
}
