using BookingApi.Models;

namespace BookingApi.Services;

// Interface for BookingService
public interface IBookingService
{
    Task<Booking> CreateAsync(Booking booking, CancellationToken ct = default); // Create new booking
    Task<Booking> UpdateTimesAsync(int id, DateTime startUtc, DateTime endUtc, byte[] rowVersion, CancellationToken ct = default); // Update booking times
    Task DeleteAsync(int id, CancellationToken ct = default); // Delete booking by id
}