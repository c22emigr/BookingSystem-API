using BookingApi.Models;
using BookingApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Services;

public sealed class BookingService(BookingDbContext db) : IBookingService
{
    // Create Booking
    public async Task<Booking> CreateAsync(Booking booking, CancellationToken ct = default)
    {
        // Validate user and resource exists
        if (!await db.Users.AnyAsync(u => u.Id == booking.UserId, ct))
            throw new ArgumentException("Invalid UserId");

        if (!await db.Resources.AnyAsync(r => r.Id == booking.ResourceId, ct))
            throw new ArgumentException("Invalid ResourceId");

        // Normalize times to UTC and validate
        (booking.StartTime, booking.EndTime) = (ToUtc(booking.StartTime), ToUtc(booking.EndTime));
        if (booking.EndTime <= booking.StartTime)
            throw new ArgumentException("EndTime must be after StartTime");

        // Check for overlapping bookings
        var hasOverLap = await db.Bookings.AsNoTracking().AnyAsync(existing =>
            existing.ResourceId == booking.ResourceId &&
            existing.StartTime < booking.EndTime &&
            booking.StartTime < existing.EndTime, ct
        );
        if (hasOverLap)
            throw new InvalidOperationException("This resource has already been booked for this time period");

        db.Bookings.Add(booking); // Add new booking to db context (EF Core)
        await db.SaveChangesAsync(ct); // Save changes
        return booking; // Return the created booking
    }

    // Update Booking
    public async Task<Booking> UpdateTimesAsync(int id, DateTime startUtc, DateTime endUtc, byte[] rowVersion, CancellationToken ct = default)
    {
        var booking = await db.Bookings.FirstOrDefaultAsync(existing => existing.Id == id, ct)
            ?? throw new KeyNotFoundException("Booking not found");

        var start = ToUtc(startUtc);
        var end = ToUtc(endUtc);
        
        if (end <= start)
            throw new ArgumentException("Booking cant end before it starts");

        // Check for overlapping bookings
        var hasOverLap = await db.Bookings.AsNoTracking().AnyAsync(existing =>
            existing.ResourceId == booking.ResourceId &&
            existing.StartTime != booking.EndTime &&
            booking.StartTime < end &&
            start < existing.EndTime, ct
        );
        if (hasOverLap)
            throw new InvalidOperationException("This resource has already been booked for this time period");

        booking.StartTime = start;
        booking.EndTime = end;

        await db.SaveChangesAsync(ct);
        return booking;
    }

    // Delete Booking
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var booking = await db.Bookings.FirstOrDefaultAsync(existing => existing.Id == id, ct)
            ?? throw new KeyNotFoundException("Booking with this Id not found");

        db.Bookings.Remove(booking);
        await db.SaveChangesAsync(ct);
    }
    // To UTC
    private static DateTime ToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
} 
