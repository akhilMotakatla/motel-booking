namespace MotelBooking.Application.DTOs.Admin;

public class DashboardStatsDto
{
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int TotalUsers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal WeeklyRevenue { get; set; }
    public double OccupancyRate { get; set; }
    public double AverageRating { get; set; }
    public List<RevenueChartData> RevenueChart { get; set; } = new();
    public List<BookingStatusData> BookingStatusChart { get; set; } = new();
}

public class RevenueChartData
{
    public string Label { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class BookingStatusData
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}
