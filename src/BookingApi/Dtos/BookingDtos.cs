// DTO for Booking Model. Represents the shape of API requests and responses.
namespace BookingApi.Dtos;

public record BookingDto(int Id, int UserId, int ResourceId, DateTime StartTime, DateTime EndTime);
public record CreateBookingDto(int UserId, int ResourceId, DateTime StartTime, DateTime EndTime);
public record UpdateBookingDto(int UserId, int ResourceId, DateTime StartTime, DateTime EndTime);