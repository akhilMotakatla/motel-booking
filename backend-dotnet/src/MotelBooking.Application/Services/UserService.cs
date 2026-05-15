using MotelBooking.Application.DTOs.User;
using MotelBooking.Application.Interfaces;
using MotelBooking.Domain.Common;
using MotelBooking.Domain.Interfaces;

namespace MotelBooking.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<UserDto>> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetWithBookingsAsync(id);
        if (user == null) return Result<UserDto>.Failure("User not found.");

        return Result<UserDto>.Success(new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            City = user.City,
            State = user.State,
            ZipCode = user.ZipCode,
            ProfileImageUrl = user.ProfileImageUrl,
            Role = user.Role,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt,
            TotalBookings = user.Bookings.Count
        });
    }

    public async Task<Result<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return Result<UserDto>.Failure("User not found.");

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Address = dto.Address;
        user.City = dto.City;
        user.State = dto.State;
        user.ZipCode = dto.ZipCode;
        user.ProfileImageUrl = dto.ProfileImageUrl;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return await GetUserByIdAsync(id);
    }

    public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role,
            IsEmailVerified = u.IsEmailVerified,
            CreatedAt = u.CreatedAt
        });
        return Result<IEnumerable<UserDto>>.Success(dtos);
    }

    public async Task<Result> DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return Result.Failure("User not found.");

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
