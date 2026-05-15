using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotelBooking.Application.DTOs.Booking;
using MotelBooking.Application.Interfaces;
using MotelBooking.Domain.Enums;

namespace MotelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService) => _bookingService = bookingService;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Customer";

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var result = await _bookingService.CreateBookingAsync(CurrentUserId, dto);
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return CreatedAtAction(nameof(GetBooking), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        var result = await _bookingService.GetBookingByIdAsync(id, CurrentUserId, CurrentUserRole);
        if (!result.IsSuccess) return NotFound(new { message = result.Error });
        return Ok(result.Data);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var result = await _bookingService.GetUserBookingsAsync(CurrentUserId);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var result = await _bookingService.CancelBookingAsync(id, CurrentUserId, CurrentUserRole);
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Booking cancelled successfully." });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllBookings(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        BookingStatus? bookingStatus = status != null && Enum.TryParse<BookingStatus>(status, out var s) ? s : null;
        var result = await _bookingService.GetAllBookingsAsync(page, pageSize, bookingStatus);
        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusRequest request)
    {
        if (!Enum.TryParse<BookingStatus>(request.Status, out var status))
            return BadRequest(new { message = "Invalid status." });

        var result = await _bookingService.UpdateBookingStatusAsync(id, status);
        if (!result.IsSuccess) return NotFound(new { message = result.Error });
        return Ok(result.Data);
    }
}

public record UpdateBookingStatusRequest(string Status);
