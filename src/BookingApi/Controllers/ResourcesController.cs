// CRUD for Resources
using AutoMapper;
using BookingApi.Dtos;
using BookingApi.Models;
using BookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ResourcesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IResourceService _service;

    // Constructor injection to get DbContext and AutoMapper
    public ResourcesController(IMapper mapper, IResourceService service)
    {
        _mapper = mapper;
        _service = service;
    }

    // ----------------
    // GET: api/resources
    // ----------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResourceDto>>> GetAll(CancellationToken ct)
    {
        var resources = await _service.GetAllAsync(ct);
        return Ok(_mapper.Map<IEnumerable<ResourceDto>>(resources)); // Map resources to DTOs and return
    }

    // ----------------
    // GET: api/resources/{id}
    // ----------------
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResourceDto>> GetById(int id, CancellationToken ct)
    {
        var resource = await _service.GetByIdAsync(id, ct);
        return resource is null ? NotFound("Resource not found")
                                : Ok(_mapper.Map<ResourceDto>(resource)); // Map to DTO and return
    }

    // ----------------
    // POST: api/resources
    // ----------------
    [HttpPost]
    public async Task<ActionResult<ResourceDto>> Create(CreateResourceDto dto)
    {
        try
        {
            var resource = _mapper.Map<Resource>(dto);
            var created = await _service.CreateAsync(resource);
            var result = _mapper.Map<ResourceDto>(created);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id }, result
            );
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); } // Resource already exists
    }

    // ----------------
    // PUT: api/resources/{id}
    // ----------------
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateResourceDto dto)
    {
        var changes = _mapper.Map<Resource>(dto);
        try
        {
            var updated = await _service.UpdateAsync(id, changes);
            return updated ? NoContent() : NotFound("Resource not found");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // ----------------
    // DELETE: api/resources/{id}
    // ----------------
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Resource not found");
        }
    }
}