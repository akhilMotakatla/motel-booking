namespace MotelBooking.Application.DTOs.Location;

public class LocationDto
{
    public int Id { get; set; }
    public string State { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

public class StateDto
{
    public string State { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
    public int BranchCount { get; set; }
}

public class CityDto
{
    public string City { get; set; } = string.Empty;
    public int BranchCount { get; set; }
}
