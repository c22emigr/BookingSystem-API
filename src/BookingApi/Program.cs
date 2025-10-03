using Microsoft.EntityFrameworkCore;
using BookingApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure EF Core with SQLite
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                        ?? "Data Source=booking.db")); // Fallback to local file

var app = builder.Build();

using (var scope = app.Services.CreateScope()) // Ensure DB is created, checks session and creates a scope
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    await db.Database.MigrateAsync(); // Sync database schema with models
}

// Enable Swagger in dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();