// CRUD for Users
using AutoMapper;
using BookingApi.Data;
using BookingApi.Dtos;
using BookingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly BookingDbContext _db;
    private readonly IMapper _mapper;

    // Constructor injection to get DbContext and AutoMapper
    public UsersController(BookingDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // --------------
    // GET: api/users
    // --------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _db.Users.ToListAsync(); // Fetch all users
        return Ok(_mapper.Map<IEnumerable<UserDto>>(users)); // Map users to DTOs and return
    }

    // ----------------
    // GET: api/users/{id}
    // ----------------
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _db.Users.FindAsync(id); // Find the user by ID
        if (user == null)
            return NotFound(); // Return 404 if user not found

        return Ok(_mapper.Map<UserDto>(user)); // Map to DTO and return
    }

    // ----------------
    // POST: api/users
    // ----------------
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        var user = _mapper.Map<User>(dto) // Map the incoming DTO to User Model

        _db.Users.Add(user); // Add new user to db context (EF Core)
        await _db.SaveChangesAsync(); // Save changes

        var result = _mapper.Map<UserDto>(user); // Map to UserDto

        return CreatedAtAction( // Return 201 created + location
            nameof(GetById),
            new { id = user.Id }, result
        );
    }

    // ----------------
    // PUT: api/users/{id}
    // ----------------
    [HttpPut("{id:int}")]
    public async Task<IActionResult<UserDto>> Update(int id, UpdateUserDto dto)
    {
        if (id <= 0)
            return BadRequest("Not a valid user ID");

        var user = await _db.Users.FindAsync(id); // Find user by ID
        if (user == null)
            return NotFound("User not found");

        // Concurrency check with RowVersion
        if (!user.RowVersion.SequenceEqual(dto.RowVersion))
            return Conflict("Concurrency conflict: User has been modified in another process");

        // Map the updated fields
        _mapper.Map(dto, user);

        await _db.SaveChangesAsync();
        return NoContent(); // 204 No Content
    }

    // ----------------
    // DELETE: api/users/{id}
    // ----------------
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id); // Find user by ID
        if (user == null)
            return NotFound("User not found"); // 404

        _db.Users.Remove(user); // Remove user from db context
        await _db.SaveChangesAsync();

        return NoContent(); // 204
    }
}

