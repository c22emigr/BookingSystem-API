 // CRUD for Users
using AutoMapper;
using BookingApi.Dtos;
using BookingApi.Models;
using BookingApi.Services;
using Microsoft.AspNetCore.Mvc;

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
        var users = await _service.GetAllAsync(ct);
        return Ok(_mapper.Map<IEnumerable<UserDto>>(users)); // Map users to DTOs and return
    }

    // ----------------
    // GET: api/users/{id}
    // ----------------
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken ct)
    {
        var user = await _service.GetByIdAsync(id, ct);

        return user is null 
            ? NotFound()
            : Ok(_mapper.Map<UserDto>(user));
    }

    // ----------------
    // POST: api/users
    // ----------------
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto, CancellationToken ct)
    {
        var user = _mapper.Map<User>(dto);
        var created = await _service.CreateAsync(user, ct);
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        var changes = _mapper.Map<User>(dto);

        await _service.UpdateAsync(id, changes, ct);
        return NoContent();
    }

    // ----------------
    // DELETE: api/users/{id}
    // ----------------
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}