using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BookingApi.Data
{
    // Used by `dotnet ef ...` at design time
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
    {
        public BookingDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var conn = config.GetConnectionString("DefaultConnection") ?? "Data Source=bookings.db";

            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseSqlite(conn)
                .Options;

            return new BookingDbContext(options);
        }
    }
}
