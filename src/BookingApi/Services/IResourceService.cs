using BookingApi.Models;

namespace BookingApi.Services;

// Interface for ResourceService
public interface IResourceService
{
    Task<IReadOnlyList<Resource>> GetAllAsync(CancellationToken ct = default);
    Task<Resource?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Resource> CreateAsync(Resource resource, CancellationToken ct = default);
    Task UpdateAsync(int id, Resource changes, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}