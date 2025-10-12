// CRUD for Bookings
using System.Data;

using AutoMapper;
using BookingApi.Data;
using BookingApi.Dtos;
using BookingApi.Models;
using BookingApi.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BookingsController : ControllerBase
{
    private readonly BookingDbContext _db;
    private readonly IMapper _mapper;
    private readonly IBookingService _service;

    // Constructor injection to get DbContext and AutoMapper
    public BookingsController(BookingDbContext db, IMapper mapper, IBookingService service)
    {
        _db = db;
        _mapper = mapper;
        _service = service;
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
        try
        {
            var booking = _mapper.Map<Booking>(dto); // Map incoming DTO to Booking Model
            var created = await _service.CreateAsync(booking);
            var result = _mapper.Map<BookingDto>(created);
            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.Id }, result
                );
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); } // Bad parameters
        catch (InvalidOperationException ex) { return Conflict(ex.Message); } // Resource already booked in this time range
    }

    // ----------------
    // PUT: api/bookings/{id}
    // ----------------
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateBookingDto dto)
    {
        try
        {
            var updated = await _service.UpdateTimesAsync(id, dto.StartTime, dto.EndTime, dto.RowVersion); // Find booking
            return Ok(_mapper.Map<BookingDto>(updated));
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Booking not found"); // 404
        }
        catch (DBConcurrencyException)
        {
            return Conflict("Concurrency conflict");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // ----------------
    // DELETE: api/bookings/{id}
    // ----------------
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id); // Find booking by ID
            return NoContent(); // 204

        }
        catch (KeyNotFoundException) { return NotFound("Booking not found"); }
    }
}