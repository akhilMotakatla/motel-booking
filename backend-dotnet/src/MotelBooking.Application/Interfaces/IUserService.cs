using MotelBooking.Application.DTOs.User;
using MotelBooking.Domain.Common;

namespace MotelBooking.Application.Interfaces;

public interface IUserService
{
    Task<Result<UserDto>> GetUserByIdAsync(int id);
    Task<Result<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync();
    Task<Result> DeleteUserAsync(int id);
}
