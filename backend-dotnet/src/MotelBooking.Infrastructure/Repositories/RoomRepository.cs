using Microsoft.EntityFrameworkCore;
using MotelBooking.Domain.Common;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Enums;
using MotelBooking.Domain.Interfaces;
using MotelBooking.Infrastructure.Data;

namespace MotelBooking.Infrastructure.Repositories;

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    public RoomRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Room?> GetWithDetailsAsync(int roomId) =>
        await _dbSet
            .Include(r => r.RoomType)
            .Include(r => r.Images.OrderBy(i => i.SortOrder))
            .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .Include(r => r.Reviews.Where(rv => rv.IsApproved))
            .FirstOrDefaultAsync(r => r.Id == roomId);

    public async Task<PagedResult<Room>> GetPagedAsync(int page, int pageSize, string? search = null,
        int? roomTypeId = null, decimal? minPrice = null, decimal? maxPrice = null,
        int? maxOccupancy = null, RoomStatus? status = null, int? locationId = null,
        string? sortBy = null, bool sortDescending = false)
    {
        var query = _dbSet
            .Include(r => r.RoomType)
            .Include(r => r.Images.Where(i => i.IsPrimary))
            .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Name.Contains(search) || r.Description.Contains(search)
                || r.RoomType.Name.Contains(search));

        if (roomTypeId.HasValue)
            query = query.Where(r => r.RoomTypeId == roomTypeId.Value);

        if (locationId.HasValue)
            query = query.Where(r => r.LocationId == locationId.Value);

        if (minPrice.HasValue)
            query = query.Where(r => r.PricePerNight >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(r => r.PricePerNight <= maxPrice.Value);

        if (maxOccupancy.HasValue)
            query = query.Where(r => r.MaxOccupancy >= maxOccupancy.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        var total = await query.CountAsync();

        IOrderedQueryable<Room> ordered = sortBy?.ToLower() switch
        {
            "rating" => sortDescending
                ? query.OrderByDescending(r => r.AverageRating)
                : query.OrderBy(r => r.AverageRating),
            "occupancy" => sortDescending
                ? query.OrderByDescending(r => r.MaxOccupancy)
                : query.OrderBy(r => r.MaxOccupancy),
            _ => sortDescending
                ? query.OrderByDescending(r => r.IsFeatured).ThenByDescending(r => r.PricePerNight)
                : query.OrderByDescending(r => r.IsFeatured).ThenBy(r => r.PricePerNight)
        };

        var items = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Room>
        {
            Items = items,
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<Room>> GetFeaturedRoomsAsync(int count = 6) =>
        await _dbSet
            .Include(r => r.RoomType)
            .Include(r => r.Images.Where(i => i.IsPrimary))
            .Where(r => r.IsFeatured && r.Status == RoomStatus.Available)
            .OrderByDescending(r => r.AverageRating)
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests = 1)
    {
        var bookedRoomIds = await _context.Set<Booking>()
            .Where(b => b.Status != BookingStatus.Cancelled
                && b.CheckInDate < checkOut && b.CheckOutDate > checkIn)
            .Select(b => b.RoomId)
            .Distinct()
            .ToListAsync();

        return await _dbSet
            .Include(r => r.RoomType)
            .Include(r => r.Images.Where(i => i.IsPrimary))
            .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .Where(r => !bookedRoomIds.Contains(r.Id)
                && r.Status == RoomStatus.Available
                && r.MaxOccupancy >= guests)
            .OrderBy(r => r.PricePerNight)
            .ToListAsync();
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
    {
        var query = _context.Set<Booking>()
            .Where(b => b.RoomId == roomId
                && b.Status != BookingStatus.Cancelled
                && b.CheckInDate < checkOut
                && b.CheckOutDate > checkIn);

        if (excludeBookingId.HasValue)
            query = query.Where(b => b.Id != excludeBookingId.Value);

        return !await query.AnyAsync();
    }

    public async Task<IEnumerable<Room>> GetByRoomTypeAsync(int roomTypeId) =>
        await _dbSet.Where(r => r.RoomTypeId == roomTypeId).ToListAsync();
}
