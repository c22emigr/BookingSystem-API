using Microsoft.EntityFrameworkCore;
using BookingApi.Models;

namespace BookingApi.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<Booking> Bookings { get; set; } = null!;
    }
}
