// DTO for User Model. Represents the shape of API requests and responses.
namespace BookingApi.Dtos;

public record UserDto(int Id, string Username, string Email, byte[] RowVersion);
public record CreateUserDto(string Username, string Email);
public record UpdateUserDto(string Username, string Email, byte[] RowVersion);