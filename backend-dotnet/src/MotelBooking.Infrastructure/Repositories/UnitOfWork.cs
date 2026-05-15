using Microsoft.EntityFrameworkCore.Storage;
using MotelBooking.Domain.Interfaces;
using MotelBooking.Infrastructure.Data;

namespace MotelBooking.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private IRoomRepository? _rooms;
    private IBookingRepository? _bookings;
    private ILocationRepository? _locations;

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRoomRepository Rooms => _rooms ??= new RoomRepository(_context);
    public IBookingRepository Bookings => _bookings ??= new BookingRepository(_context);
    public ILocationRepository Locations => _locations ??= new LocationRepository(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync() =>
        _transaction = await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        await _transaction!.CommitAsync();
        _transaction.Dispose();
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction!.RollbackAsync();
        _transaction.Dispose();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
