using Microsoft.AspNetCore.Mvc;
using MotelBooking.Application.Interfaces;

namespace MotelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService) =>
        _locationService = locationService;

    /// <summary>Returns all US states that have active branches.</summary>
    [HttpGet("states")]
    public async Task<IActionResult> GetStates()
    {
        var result = await _locationService.GetStatesAsync();
        return Ok(result.Data);
    }

    /// <summary>Returns cities within a state that have active branches.</summary>
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities([FromQuery] string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return BadRequest(new { message = "State is required." });

        var result = await _locationService.GetCitiesByStateAsync(state);
        return Ok(result.Data);
    }

    /// <summary>Returns branches within a state+city.</summary>
    [HttpGet("branches")]
    public async Task<IActionResult> GetBranches([FromQuery] string state, [FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(city))
            return BadRequest(new { message = "State and city are required." });

        var result = await _locationService.GetBranchesByCityAsync(state, city);
        return Ok(result.Data);
    }

    /// <summary>Returns a single branch by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBranch(int id)
    {
        var result = await _locationService.GetBranchByIdAsync(id);
        if (!result.IsSuccess) return NotFound(new { message = result.Error });
        return Ok(result.Data);
    }
}
