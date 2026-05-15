using Microsoft.Extensions.DependencyInjection;
using MotelBooking.Application.Interfaces;
using MotelBooking.Application.Services;

namespace MotelBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ILocationService, LocationService>();
        return services;
    }
}
