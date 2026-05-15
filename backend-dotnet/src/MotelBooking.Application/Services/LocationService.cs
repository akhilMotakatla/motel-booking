using MotelBooking.Application.DTOs.Location;
using MotelBooking.Application.Interfaces;
using MotelBooking.Domain.Common;
using MotelBooking.Domain.Interfaces;

namespace MotelBooking.Application.Services;

public class LocationService : ILocationService
{
    private readonly IUnitOfWork _uow;
    public LocationService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<StateDto>>> GetStatesAsync()
    {
        var states = await _uow.Locations.GetStatesAsync();
        var all = await _uow.Locations.GetAllAsync();
        var dtos = states.Select(s => new StateDto
        {
            State = s,
            StateCode = all.FirstOrDefault(l => l.State == s)?.StateCode ?? "",
            BranchCount = all.Count(l => l.State == s && l.IsActive)
        }).OrderBy(s => s.State);
        return Result<IEnumerable<StateDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<CityDto>>> GetCitiesByStateAsync(string state)
    {
        var cities = await _uow.Locations.GetCitiesByStateAsync(state);
        var all = await _uow.Locations.FindAsync(l => l.State == state && l.IsActive);
        var dtos = cities.Select(c => new CityDto
        {
            City = c,
            BranchCount = all.Count(l => l.City == c)
        }).OrderBy(c => c.City);
        return Result<IEnumerable<CityDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<LocationDto>>> GetBranchesByCityAsync(string state, string city)
    {
        var branches = await _uow.Locations.GetBranchesByCityAsync(state, city);
        return Result<IEnumerable<LocationDto>>.Success(branches.Select(Map).OrderBy(b => b.BranchName));
    }

    public async Task<Result<LocationDto>> GetBranchByIdAsync(int id)
    {
        var loc = await _uow.Locations.GetByIdAsync(id);
        if (loc == null) return Result<LocationDto>.Failure("Branch not found.");
        return Result<LocationDto>.Success(Map(loc));
    }

    private static LocationDto Map(Domain.Entities.Location l) => new()
    {
        Id = l.Id,
        State = l.State,
        StateCode = l.StateCode,
        City = l.City,
        BranchName = l.BranchName,
        BranchCode = l.BranchCode,
        Address = l.Address,
        PhoneNumber = l.PhoneNumber,
        Email = l.Email,
        IsActive = l.IsActive
    };
}
