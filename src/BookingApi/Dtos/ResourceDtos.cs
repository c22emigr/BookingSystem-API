// DTO for Resource Model. Represents the shape of API requests and responses.
namespace BookingApi.Dtos;

public record ResourceDto(int Id, string Name, string Description, byte[] RowVersion);
public record CreateResourceDto(string Name, string Description);
public record UpdateResourceDto(string Name, string Description, byte[] RowVersion);