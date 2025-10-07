using BookingApi.Models;
using BookingApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Services;

public class BookingService(BookingDbContext db) : InterfaceBookingService
{
    public async Task<Booking> CreateAsync(Booking booking, CancellationToken ct = default)
    {
        // Validate user and resource exists
        if (!await db.Users.AnyAsync(u => u.Id == booking.UserId, ct))
            throw new ArgumentException("Invalid UserId");

        if (!await db.Resources.AnyAsync(r => r.Id == booking.ResourceId, ct))
            throw new ArgumentException("Invalid ResourceId");

        // Normalize times to UTC and validate
        (booking.StartTime, booking.Endtime) = (ToUtc(booking.StartTime), ToUtc(booking.EndTime));
        if (booking.EndTime <= booking.StartTime)
            throw new ArgumentException("EndTime must be after StartTime");

        // Check for overlapping bookings
        var hasOverLap = await db.Bookings.AsNoTracking().AnyAsync(newBooking =>
            booking.ResourceId == booking.ResourceId &&
            newBooking.StartTime < booking.EndTime &&
            booking.StartTime < newBooking.EndTime, ct
        );
        if (hasOverLap)
            throw new InvalidOperationException("This resource has already been booked for this time period");

        db.Bookings.Add(booking); // Add new booking to db context (EF Core)
        await db.SaveChangesAsync(ct); // Save changes
        return booking; // Return the created booking
    }
} 
