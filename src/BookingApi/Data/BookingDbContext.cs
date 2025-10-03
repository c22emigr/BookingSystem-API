using Microsoft.EntityFrameworkCore;
using BookingApi.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

// DbContext for the Booking system to interact with the database

namespace BookingApi.Data
{
    public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>(); // User table
        public DbSet<Resource> Resources => Set<Resource>(); // Resource table
        public DbSet<Booking> Bookings => Set<Booking>(); // Booking table


        protected override void OnModelCreating(ModelBuilder b) // Only ER Core calls this on initialization
        {
            b.Entity<User>()
                .HasIndex(u => u.Email) // Ensure email uniqueness
                .IsUnique();

            b.Entity<Resource>()
                .HasIndex(r => r.Name)
                .IsUnique(); // Resource names are unique

            b.Entity<Booking>()
                .HasIndex(x => new { x.ResourceId, x.StartTime, x.EndTime }); // Db index

            foreach (var p in b.Model.GetEntityTypes() // Ensure date storage as UTC
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(DateTime)))
            {
                p.SetValueConverter (new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc), // To database
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)  // From database
                ));
            }
        
        base.OnModelCreating(b); // Call base method
        }
    }
}
