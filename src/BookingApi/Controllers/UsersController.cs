 // CRUD for Users
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
public class UsersController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IUserService _service;

    // Constructor injection to get DbContext and AutoMapper
    public UsersController(IMapper mapper, IUserService service)
    {
        _mapper = mapper;
        _service = service;
    }

    // --------------
    // GET: api/users
    // --------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken ct)
    {
        var users = await _service.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<UserDto>>(users)); // Map users to DTOs and return
    }

    // ----------------
    // GET: api/users/{id}
    // ----------------
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken ct)
    {
        var user = await _service.GetByIdAsync(id, ct);
        return user is null ? NotFound()
                            : Ok(_mapper.Map<UserDto>(user));
    }

    // ----------------
    // POST: api/users
    // ----------------
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        var user = _mapper.Map<User>(dto);
        var created = await _service.CreateAsync(user);
        var result = _mapper.Map<UserDto>(created);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id }, result
        );
    }

    // ----------------
    // PUT: api/users/{id}
    // ----------------
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto dto)
    {
        var changes = _mapper.Map<User>(dto);
        try
        {
            var updated = await _service.UpdateAsync(id, changes);
            return updated ? NoContent() : NotFound("User not found");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("User not found");
        }
    }

    // ----------------
    // DELETE: api/users/{id}
    // ----------------
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound("User not found"); }
    }
}