using MotelBooking.Domain.Entities;

namespace MotelBooking.Domain.Interfaces;

public interface ILocationRepository : IGenericRepository<Location>
{
    Task<IEnumerable<string>> GetStatesAsync();
    Task<IEnumerable<string>> GetCitiesByStateAsync(string state);
    Task<IEnumerable<Location>> GetBranchesByCityAsync(string state, string city);
    Task<Location?> GetByBranchCodeAsync(string branchCode);
}
