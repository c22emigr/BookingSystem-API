using System.ComponentModel.DataAnnotations;

namespace BookingApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required, EmailAddress, MaxLength(250)]
        public string Email { get; set; } = null!;

        [Timestamp] // Optimistic concurrency token to handle concurrent updates
        public byte[]? RowVersion { get; set; } // nullable temporary
    }
}