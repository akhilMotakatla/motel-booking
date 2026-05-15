using MotelBooking.Domain.Entities;

namespace MotelBooking.Application.Interfaces;

public interface IEmailService
{
    Task SendBookingConfirmationAsync(Booking booking);
    Task SendWelcomeEmailAsync(User user);
}
