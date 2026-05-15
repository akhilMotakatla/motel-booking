using MotelBooking.Application.DTOs.Admin;
using MotelBooking.Application.Interfaces;
using MotelBooking.Domain.Common;
using MotelBooking.Domain.Enums;
using MotelBooking.Domain.Interfaces;

namespace MotelBooking.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<DashboardStatsDto>> GetDashboardStatsAsync()
    {
        var allBookings = await _unitOfWork.Bookings.GetAllPagedAsync(1, int.MaxValue, null);
        var allRooms = await _unitOfWork.Rooms.GetAllAsync();
        var allUsers = await _unitOfWork.Users.GetAllAsync();

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var weekStart = now.AddDays(-(int)now.DayOfWeek);

        var bookingsList = allBookings.Items.ToList();
        var roomsList = allRooms.ToList();

        var stats = new DashboardStatsDto
        {
            TotalRooms = roomsList.Count,
            AvailableRooms = roomsList.Count(r => r.Status == RoomStatus.Available),
            OccupiedRooms = roomsList.Count(r => r.Status == RoomStatus.Occupied),
            TotalBookings = bookingsList.Count,
            PendingBookings = bookingsList.Count(b => b.Status == BookingStatus.Pending),
            ConfirmedBookings = bookingsList.Count(b => b.Status == BookingStatus.Confirmed),
            TotalUsers = allUsers.Count(),
            TotalRevenue = await _unitOfWork.Bookings.GetTotalRevenueAsync(),
            MonthlyRevenue = await _unitOfWork.Bookings.GetTotalRevenueAsync(monthStart, now),
            WeeklyRevenue = await _unitOfWork.Bookings.GetTotalRevenueAsync(weekStart, now),
            OccupancyRate = roomsList.Count > 0
                ? Math.Round((double)roomsList.Count(r => r.Status == RoomStatus.Occupied) / roomsList.Count * 100, 1)
                : 0,
            RevenueChart = GenerateRevenueChart(bookingsList),
            BookingStatusChart = Enum.GetValues<BookingStatus>()
                .Select(s => new BookingStatusData
                {
                    Status = s.ToString(),
                    Count = bookingsList.Count(b => b.Status == s)
                }).ToList()
        };

        return Result<DashboardStatsDto>.Success(stats);
    }

    private static List<RevenueChartData> GenerateRevenueChart(
        List<Domain.Entities.Booking> bookings)
    {
        var result = new List<RevenueChartData>();
        for (int i = 5; i >= 0; i--)
        {
            var month = DateTime.UtcNow.AddMonths(-i);
            var revenue = bookings
                .Where(b => b.CreatedAt.Year == month.Year && b.CreatedAt.Month == month.Month
                    && b.Status != BookingStatus.Cancelled)
                .Sum(b => b.TotalAmount);
            result.Add(new RevenueChartData
            {
                Label = month.ToString("MMM yyyy"),
                Amount = revenue
            });
        }
        return result;
    }
}
