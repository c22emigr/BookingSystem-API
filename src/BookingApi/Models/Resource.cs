using System.ComponentModel.DataAnnotations;

namespace BookingApi.Models
{
    public class Resource
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, EmailAddress, MaxLength(250)]
        public string Email { get; set; } = null!;

        [Timestamp] // Optimistic concurrency token to handle concurrent updates
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}