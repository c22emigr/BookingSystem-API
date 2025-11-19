using BookingApi.Models;
using BookingApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Services;

public sealed class ResourceService(BookingDbContext db) : IResourceService
{
    public async Task<IReadOnlyList<Resource>> GetAllAsync(CancellationToken ct = default) =>
        await db.Resources.AsNoTracking().ToListAsync(ct);

    public async Task<Resource?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await db.Resources.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);


    // Create Resource
    public async Task<Resource> CreateAsync(Resource resource, CancellationToken ct = default)
    {
        // Checks if name already exists
        var exists = await db.Resources.AsNoTracking()
            .AnyAsync(r => r.Name == resource.Name, ct);
        if (exists)
            throw new InvalidOperationException("A resource with this name already exists");

        db.Resources.Add(resource);
        await db.SaveChangesAsync(ct);
        return resource;
    }

    public async Task UpdateAsync(int id, Resource changes, CancellationToken ct = default)
    {
        var resource = await db.Resources.FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new KeyNotFoundException("Resource not found");

        if (!string.Equals(resource.Name, changes.Name, StringComparison.Ordinal))
        {
            var nameTaken = await db.Resources.AsNoTracking()
                .AnyAsync(r => r.Id != id && r.Name == changes.Name, ct);
            if (nameTaken) 
                throw new InvalidOperationException("A resource with this name already exists");
            resource.Name = changes.Name;
        }

        resource.Description = changes.Description;

        await db.SaveChangesAsync(ct);
    }
    
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var resource = await db.Resources.FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new KeyNotFoundException("Resource not found");

        db.Resources.Remove(resource);
        await db.SaveChangesAsync(ct);
    }
}