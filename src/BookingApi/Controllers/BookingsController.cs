// CRUD for Bookings
using AutoMapper;
using BookingApi.Dtos;
using BookingApi.Models;
using BookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BookingsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBookingService _service;

    // Constructor injection to get DbContext and AutoMapper
    public BookingsController(IMapper mapper, IBookingService service)
    {
        _mapper = mapper;
        _service = service;
    }

    // ----------------
    // GET: api/bookings
    // ----------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll(CancellationToken ct)
    {
        var booking = await _service.GetAllAsync(ct);
        return Ok(_mapper.Map<IEnumerable<BookingDto>>(booking)); // Map bookings to DTOs and return
    }

    // ----------------
    // GET: api/bookings/{id}
    // ----------------
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id, CancellationToken ct)
    {
        var booking = await _service.GetByIdAsync(id, ct);

        return booking is null 
            ? NotFound("Booking not found")
            : Ok(_mapper.Map<BookingDto>(booking));

    }

    // ----------------
    // POST: api/bookings
    // ----------------
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        var booking = _mapper.Map<Booking>(dto);
        var created = await _service.CreateAsync(booking, ct);

        var result = _mapper.Map<BookingDto>(created);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id }, result
        );
    }

    // ----------------
    // PUT: api/bookings/{id}
    // ----------------
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateTimesAsync(id, dto.StartTime, dto.EndTime, dto.RowVersion, ct);
        return Ok(_mapper.Map<BookingDto>(updated));
    }

    // ----------------
    // DELETE: api/bookings/{id}
    // ----------------
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}