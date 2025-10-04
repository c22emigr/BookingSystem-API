// DTO for Resource Model. Represents the shape of API requests and responses.
namespace BookingApi.Dtos;

public record ResourceDto(int Id, string Name, string Email);
public record CreateResourceDto(string Name, string Email);
public record UpdateResourceDto(string Name, string Email);