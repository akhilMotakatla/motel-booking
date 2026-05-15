using MotelBooking.Application.DTOs.Location;
using MotelBooking.Domain.Common;

namespace MotelBooking.Application.Interfaces;

public interface ILocationService
{
    Task<Result<IEnumerable<StateDto>>> GetStatesAsync();
    Task<Result<IEnumerable<CityDto>>> GetCitiesByStateAsync(string state);
    Task<Result<IEnumerable<LocationDto>>> GetBranchesByCityAsync(string state, string city);
    Task<Result<LocationDto>> GetBranchByIdAsync(int id);
}
