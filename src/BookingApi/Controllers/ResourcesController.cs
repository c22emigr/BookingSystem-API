// CRUD for Resources
using AutoMapper;
using BookingApi.Data;
using BookingApi.Dtos;
using BookingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ResourcesController : ControllerBase
{
    private readonly BookingDbContext _db;
    private readonly IMapper _mapper;

    // Constructor injection to get DbContext and AutoMapper
    public ResourcesController(BookingDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // ----------------
    // GET: api/resources
    // ----------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResourceDto>>> GetAll()
    {
        var resources = await _db.Resources.AsNoTracking().ToListAsync(); // Fetch all resources
        return Ok(_mapper.Map<IEnumerable<ResourceDto>>(resources)); // Map resources to DTOs and return
    }

    // ----------------
    // GET: api/resources/{id}
    // ----------------
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResourceDto>> GetById(int id)
    {
        var resource = await _db.Resources.FindAsync(id); // Find resource by id
        if (resource == null)
            return NotFound("Resource not found"); // 404

        return Ok(_mapper.Map<ResourceDto>(resource)); // Map to DTO and return
    }

    // ----------------
    // POST: api/resources
    // ----------------
    [HttpPost]
    public async Task<ActionResult<ResourceDto>> Create(CreateResourceDto dto)
    {
        var resource = _mapper.Map<Resource>(dto); // Map incoming DTO to Resource model

        _db.Resources.Add(resource); // Add new resource to db context (EF Core)
        await _db.SaveChangesAsync();

        var result = _mapper.Map<ResourceDto>(resource); // Map to ResourceDto

        return CreatedAtAction( // 201 with location header
            nameof(GetById),
            new { id = resource.Id }, result
        );
    }

    // ----------------
    // PUT: api/resources/{id}
    // ----------------
    [HttpPut("{int:id}")]
    public async Task<IActionResult> Update(int id, UpdateResourceDto dto)
    {
        if (id <= 0)
            return BadRequest("Invalid resource ID"); // 400

        var resource = await _db.Resources.FindAsync(id); // Find resource by ID
        if (resource == null)
            return NotFound("Resource not found"); // 404

        // Concurrency check with RowVersion
        if (!resource.RowVersion.SequenceEqual(dto.RowVersion))
            return Conflict("Concurrency conflict: User has been modified in another process");

        // Map updated fields from DTO to Model
        _mapper.Map(dto, resource);

        await _db.SaveChangesAsync();

        return NoContent(); // 204
    }

    // ----------------
    // DELETE: api/resources/{id}
    // ----------------
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resource = await _db.Resources.FindAsync(id); // Find resource by ID
        if (resource == null)
            return NotFound("Resource not found"); // 404

        _db.Resources.Remove(resource); // Remove resource from db context
        await _db.SaveChangesAsync();

        return NoContent(); // 204
    }
}