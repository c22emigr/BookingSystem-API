// CRUD for Bookings
using AutoMapper;
using BookingApi.Data;
using BookingApi.Dtos;
using BookingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BookingsController : ControllerBase
{
    private readonly BookingDbContext _db;
    private readonly IMapper _mapper;

    // Constructor injection to get DbContext and AutoMapper
    public BookingsController(BookingDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // ----------------
    // GET: api/bookings
    // ----------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
    {
        var booking = await _db.Bookings
            .AsNoTracking()
            .Include(b => b.User) // Include user for booking
            .Include(b => b.Resource) // Include resource for booking
            .ToListAsync(); // Fetch all bookings
        return Ok(_mapper.Map<IEnumerable<BookingDto>>(booking)); // Map bookings to DTOs and return
    }

    // ----------------
    // GET: api/bookings/{id}
    // ----------------
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        var booking = await _db.Bookings
            .AsNoTracking()
            .Include(b => b.User) // Include user for booking
            .Include(b => b.Resource) // Include resource for booking
            .FirstOrDefaultAsync(b => b.Id == id); // Find booking by id
        if (booking == null)
            return NotFound("Booking not found"); // 404

        return Ok(_mapper.Map<BookingDto>(booking)); // Map to DTO and return
    }

    // ----------------
    // POST: api/bookings
    // ----------------
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingDto dto)
    {
        var booking = _mapper.Map<Booking>(dto); // Map incoming DTO to Booking Model

        // Check for overlapping bookings for the same resource
        var hasOverLap = await _db.Bookings.AnyAsync(b =>
            b.ResourceId == booking.ResourceId &&
            b.StartTime < booking.EndTime &&
            booking.StartTime < b.EndTime
        );
        if (hasOverLap)
            return Conflict("The resource is already booked for the specified time range."); // 409 Conflict

        _db.Bookings.Add(booking); // Add new booking to db context (EF Core)
        await _db.SaveChangesAsync();

        var result = _mapper.Map<BookingDto>(booking); // Map to BookingDto

        return CreatedAtAction( // 201 with location header
            nameof(GetById),
            new { id = booking.Id }, result
        );
    }

    // ----------------
    // PUT: api/bookings/{id}
    // ----------------
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateBookingDto dto)
    {
        if (id <= 0)
            return BadRequest("Invalid booking ID"); // 400

        var booking = await _db.Bookings.FindAsync(id); // Find booking by ID
        if (booking == null)
            return NotFound("Booking not found"); // 404

        // Concurrency check with RowVersion
        if (!booking.RowVersion.SequenceEqual(dto.RowVersion))
            return Conflict("Concurrency conflict: Booking has been modified in another process");

        // Map updated fields from DTO to Model
        _mapper.Map(dto, booking);

        await _db.SaveChangesAsync();
        return NoContent(); // 204
    }

    // ----------------
    // DELETE: api/bookings/{id}
    // ----------------
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await _db.Bookings.FindAsync(id); // Find booking by ID
        if (booking == null)
            return NotFound("Booking not found"); // 404

        _db.Bookings.Remove(booking); // Remove booking from db context
        await _db.SaveChangesAsync();

        return NoContent(); // 204
    }
}