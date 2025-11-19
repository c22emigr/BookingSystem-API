using BookingApi.Models;

namespace BookingApi.Services;

// Interface for UserService
public interface IUserService
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(int id, User changes, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}