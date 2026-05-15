using MotelBooking.Application.DTOs.Admin;
using MotelBooking.Domain.Common;

namespace MotelBooking.Application.Interfaces;

public interface IAdminService
{
    Task<Result<DashboardStatsDto>> GetDashboardStatsAsync();
}
