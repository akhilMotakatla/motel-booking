using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MotelBooking.Application.Interfaces;
using MotelBooking.Domain.Entities;

namespace MotelBooking.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendBookingConfirmationAsync(Booking booking)
    {
        var user = booking.User;
        var room = booking.Room;
        var location = booking.Location;

        if (user == null || string.IsNullOrWhiteSpace(user.Email))
        {
            _logger.LogWarning("Cannot send confirmation — booking {Id} has no user email.", booking.Id);
            return;
        }

        var locationLine = location != null
            ? $"{location.BranchName}, {location.Address}"
            : "Starry Nights Motel";

        var html = BuildBookingEmailHtml(
            guestName: user.FullName,
            confirmationNumber: booking.ConfirmationNumber ?? $"MB{booking.Id}",
            roomName: room?.Name ?? "Premium Room",
            locationLine: locationLine,
            checkIn: booking.CheckInDate.ToString("dddd, MMMM d, yyyy"),
            checkOut: booking.CheckOutDate.ToString("dddd, MMMM d, yyyy"),
            nights: booking.NightsCount,
            guests: booking.NumberOfGuests,
            subtotal: booking.TotalAmount - booking.TaxAmount,
            tax: booking.TaxAmount,
            total: booking.TotalAmount,
            specialRequests: booking.SpecialRequests
        );

        await SendEmailAsync(user.Email, user.FullName,
            $"✦ Booking Confirmed — #{booking.ConfirmationNumber}", html);
    }

    public async Task SendWelcomeEmailAsync(User user)
    {
        var html = $@"
        <div style='font-family:Inter,sans-serif;max-width:600px;margin:0 auto;background:#0a0a0f;color:#f8f8f8;padding:40px;border-radius:16px;'>
            <h2 style='color:#d4af37;font-family:Georgia,serif;'>Welcome to Starry Nights Motel, {user.FullName}!</h2>
            <p style='color:rgba(248,248,248,0.7);'>Your account has been successfully created. You can now browse our premium rooms and make reservations across our 3,000+ locations nationwide.</p>
            <a href='http://localhost:4200/rooms' style='display:inline-block;margin-top:20px;padding:14px 32px;background:linear-gradient(135deg,#f5c842,#d4af37);color:#0a0a0f;text-decoration:none;border-radius:8px;font-weight:700;'>Explore Rooms</a>
        </div>";

        await SendEmailAsync(user.Email, user.FullName, "Welcome to Starry Nights Motel!", html);
    }

    private async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        var host = _config["Email:SmtpHost"];
        var user = _config["Email:SmtpUser"];
        var pass = _config["Email:SmtpPassword"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
            _logger.LogInformation(
                "[EMAIL-SKIPPED] To: {To} | Subject: {Subject} | SMTP not configured.", toEmail, subject);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _config["Email:FromName"] ?? "Starry Nights Motel",
            _config["Email:FromEmail"] ?? user));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        var body = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = body.ToMessageBody();

        using var smtp = new SmtpClient();
        try
        {
            var port = int.Parse(_config["Email:SmtpPort"] ?? "587");
            await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(user, pass);
            await smtp.SendAsync(message);
            _logger.LogInformation("Confirmation email sent to {Email}", toEmail);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }

    private static string BuildBookingEmailHtml(
        string guestName, string confirmationNumber, string roomName,
        string locationLine, string checkIn, string checkOut,
        int nights, int guests, decimal subtotal, decimal tax, decimal total,
        string? specialRequests)
    {
        var requests = string.IsNullOrWhiteSpace(specialRequests)
            ? ""
            : $"<tr><td style='padding:12px 0;color:rgba(248,248,248,0.5);font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06)'>Special Requests</td><td style='padding:12px 0;font-weight:600;text-align:right;font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06)'>{specialRequests}</td></tr>";

        return $@"<!DOCTYPE html>
<html lang='en'>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width,initial-scale=1'></head>
<body style='margin:0;padding:0;background:#0d0d16;font-family:Inter,-apple-system,sans-serif;'>
<table width='100%' cellpadding='0' cellspacing='0' style='background:#0d0d16;min-height:100vh;'>
<tr><td align='center' style='padding:40px 16px;'>
<table width='600' cellpadding='0' cellspacing='0' style='max-width:600px;width:100%;'>

  <!-- Header -->
  <tr><td style='background:linear-gradient(135deg,#1a1a0a 0%,#0a0a0f 100%);border-radius:20px 20px 0 0;padding:40px 40px 32px;text-align:center;border:1px solid rgba(212,175,55,0.2);border-bottom:none;'>
    <div style='font-size:32px;margin-bottom:8px;'>✦</div>
    <h1 style='margin:0;font-family:Georgia,serif;font-size:26px;font-weight:900;background:linear-gradient(135deg,#f5c842,#d4af37);-webkit-background-clip:text;-webkit-text-fill-color:transparent;background-clip:text;'>Booking Confirmed!</h1>
    <p style='margin:8px 0 0;color:rgba(248,248,248,0.55);font-size:14px;'>Your reservation is all set. We can't wait to welcome you.</p>
  </td></tr>

  <!-- Confirmation Banner -->
  <tr><td style='background:rgba(212,175,55,0.08);border:1px solid rgba(212,175,55,0.25);border-top:none;border-bottom:none;padding:20px 40px;text-align:center;'>
    <p style='margin:0;font-size:13px;color:rgba(212,175,55,0.7);text-transform:uppercase;letter-spacing:0.1em;'>Confirmation Number</p>
    <p style='margin:4px 0 0;font-size:22px;font-weight:900;color:#f5c842;font-family:''Courier New'',monospace;letter-spacing:0.08em;'>{confirmationNumber}</p>
  </td></tr>

  <!-- Guest Greeting -->
  <tr><td style='background:#111118;border:1px solid rgba(255,255,255,0.06);border-top:none;border-bottom:none;padding:32px 40px;'>
    <p style='margin:0 0 24px;font-size:16px;color:rgba(248,248,248,0.85);'>Dear <strong style='color:#f8f8f8;'>{guestName}</strong>,</p>
    <p style='margin:0;font-size:14px;color:rgba(248,248,248,0.6);line-height:1.7;'>Thank you for choosing <strong style='color:#d4af37;'>Starry Nights Motel</strong>. Your booking has been confirmed and we're preparing everything to make your stay exceptional.</p>
  </td></tr>

  <!-- Booking Details -->
  <tr><td style='background:#111118;border:1px solid rgba(255,255,255,0.06);border-top:none;border-bottom:none;padding:0 40px 32px;'>
    <h3 style='margin:0 0 16px;font-family:Georgia,serif;font-size:16px;color:#d4af37;'>Reservation Details</h3>
    <table width='100%' cellpadding='0' cellspacing='0'>
      <tr><td style='padding:12px 0;color:rgba(248,248,248,0.5);font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);'>Room</td>
          <td style='padding:12px 0;font-weight:700;text-align:right;font-size:13px;color:#f8f8f8;border-bottom:1px solid rgba(255,255,255,0.06);'>{roomName}</td></tr>
      <tr><td style='padding:12px 0;color:rgba(248,248,248,0.5);font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);'>Location</td>
          <td style='padding:12px 0;font-weight:600;text-align:right;font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);color:#f8f8f8;'>{locationLine}</td></tr>
      <tr><td style='padding:12px 0;color:rgba(248,248,248,0.5);font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);'>Check-In</td>
          <td style='padding:12px 0;font-weight:600;text-align:right;font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);color:#f8f8f8;'>{checkIn}</td></tr>
      <tr><td style='padding:12px 0;color:rgba(248,248,248,0.5);font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);'>Check-Out</td>
          <td style='padding:12px 0;font-weight:600;text-align:right;font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);color:#f8f8f8;'>{checkOut}</td></tr>
      <tr><td style='padding:12px 0;color:rgba(248,248,248,0.5);font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);'>Duration</td>
          <td style='padding:12px 0;font-weight:600;text-align:right;font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);color:#f8f8f8;'>{nights} night{(nights != 1 ? "s" : "")}</td></tr>
      <tr><td style='padding:12px 0;color:rgba(248,248,248,0.5);font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);'>Guests</td>
          <td style='padding:12px 0;font-weight:600;text-align:right;font-size:13px;border-bottom:1px solid rgba(255,255,255,0.06);color:#f8f8f8;'>{guests}</td></tr>
      {requests}
    </table>
  </td></tr>

  <!-- Price Breakdown -->
  <tr><td style='background:rgba(212,175,55,0.04);border:1px solid rgba(212,175,55,0.15);border-top:none;border-bottom:none;padding:24px 40px;'>
    <h3 style='margin:0 0 16px;font-family:Georgia,serif;font-size:16px;color:#d4af37;'>Price Summary</h3>
    <table width='100%' cellpadding='0' cellspacing='0'>
      <tr><td style='padding:8px 0;color:rgba(248,248,248,0.55);font-size:13px;'>Subtotal ({nights} night{(nights != 1 ? "s" : "")})</td>
          <td style='padding:8px 0;text-align:right;font-size:13px;color:rgba(248,248,248,0.7);'>${subtotal:F2}</td></tr>
      <tr><td style='padding:8px 0;color:rgba(248,248,248,0.55);font-size:13px;'>Taxes &amp; Fees (12%)</td>
          <td style='padding:8px 0;text-align:right;font-size:13px;color:rgba(248,248,248,0.7);'>${tax:F2}</td></tr>
      <tr><td style='padding:14px 0 8px;font-weight:800;font-size:15px;color:#f8f8f8;border-top:1px solid rgba(255,255,255,0.1);'>Total Charged</td>
          <td style='padding:14px 0 8px;text-align:right;font-weight:900;font-size:18px;color:#f5c842;border-top:1px solid rgba(255,255,255,0.1);'>${total:F2}</td></tr>
    </table>
  </td></tr>

  <!-- Policies -->
  <tr><td style='background:#111118;border:1px solid rgba(255,255,255,0.06);border-top:none;border-bottom:none;padding:24px 40px;'>
    <h3 style='margin:0 0 12px;font-size:13px;color:rgba(248,248,248,0.4);text-transform:uppercase;letter-spacing:0.1em;'>Important Information</h3>
    <p style='margin:0 0 8px;font-size:13px;color:rgba(248,248,248,0.55);'>✓ &nbsp;Free cancellation up to 24 hours before check-in</p>
    <p style='margin:0 0 8px;font-size:13px;color:rgba(248,248,248,0.55);'>✓ &nbsp;Payment collected at the property</p>
    <p style='margin:0;font-size:13px;color:rgba(248,248,248,0.55);'>✓ &nbsp;24/7 front desk available — call us anytime</p>
  </td></tr>

  <!-- CTA -->
  <tr><td style='background:#111118;border:1px solid rgba(255,255,255,0.06);border-top:none;border-bottom:none;padding:24px 40px;text-align:center;'>
    <a href='http://localhost:4200/dashboard' style='display:inline-block;padding:14px 36px;background:linear-gradient(135deg,#f5c842,#d4af37);color:#0a0a0f;text-decoration:none;border-radius:10px;font-weight:800;font-size:14px;letter-spacing:0.04em;'>View My Booking</a>
  </td></tr>

  <!-- Footer -->
  <tr><td style='background:#080810;border:1px solid rgba(255,255,255,0.06);border-top:none;border-radius:0 0 20px 20px;padding:24px 40px;text-align:center;'>
    <p style='margin:0 0 6px;color:#d4af37;font-size:16px;'>✦ Starry Nights Motel</p>
    <p style='margin:0 0 6px;font-size:12px;color:rgba(248,248,248,0.3);'>1-800-STARRY-N &nbsp;|&nbsp; hello@starrynightsmotel.com</p>
    <p style='margin:0;font-size:11px;color:rgba(248,248,248,0.2);'>This is an automated confirmation. Please do not reply to this email.</p>
  </td></tr>

</table>
</td></tr>
</table>
</body>
</html>";
    }
}
