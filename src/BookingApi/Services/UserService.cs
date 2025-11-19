using BookingApi.Models;
using BookingApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Services;

public sealed class UserService(BookingDbContext db) : IUserService
{
    // Get all users
    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        await db.Users.AsNoTracking().ToListAsync(ct);

    // Get user by id
    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        
    //Create User
    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        var exists = await db.Users.AsNoTracking()
            .AnyAsync(u => u.Email == user.Email, ct);

        if (exists)
            throw new InvalidOperationException("This email is already registered to a user");

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return user;
    }

    // Update User
    public async Task UpdateAsync(int id, User changes, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new KeyNotFoundException("User not found");

        if (!string.Equals(user.Email, changes.Email, StringComparison.Ordinal))
        {
            var emailTaken = await db.Users.AsNoTracking()
                .AnyAsync(u => u.Id != id && u.Email == changes.Email, ct);
            if (emailTaken)
                throw new InvalidOperationException("A user with this email already exists");
            user.Email = changes.Email;
        }

        user.Username = changes.Username;

        await db.SaveChangesAsync(ct);
    }

    // Delete User
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new KeyNotFoundException("User not found");

        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }
}