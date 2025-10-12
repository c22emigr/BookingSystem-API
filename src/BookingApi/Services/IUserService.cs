using BookingApi.Models;

namespace BookingApi.Services;

// Interface for UserService
public interface IUserService
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, User changes, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}