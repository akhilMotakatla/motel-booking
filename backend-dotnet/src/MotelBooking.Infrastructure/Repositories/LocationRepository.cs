using Microsoft.EntityFrameworkCore;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Interfaces;
using MotelBooking.Infrastructure.Data;

namespace MotelBooking.Infrastructure.Repositories;

public class LocationRepository : GenericRepository<Location>, ILocationRepository
{
    public LocationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<string>> GetStatesAsync() =>
        await _dbSet
            .Where(l => l.IsActive)
            .Select(l => l.State)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

    public async Task<IEnumerable<string>> GetCitiesByStateAsync(string state) =>
        await _dbSet
            .Where(l => l.State == state && l.IsActive)
            .Select(l => l.City)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

    public async Task<IEnumerable<Location>> GetBranchesByCityAsync(string state, string city) =>
        await _dbSet
            .Where(l => l.State == state && l.City == city && l.IsActive)
            .OrderBy(l => l.BranchName)
            .ToListAsync();

    public async Task<Location?> GetByBranchCodeAsync(string branchCode) =>
        await _dbSet.FirstOrDefaultAsync(l => l.BranchCode == branchCode);
}
