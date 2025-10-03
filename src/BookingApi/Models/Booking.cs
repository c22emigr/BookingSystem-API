using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApi.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }  // UTC

        [Required]
        public DateTime EndTime { get; set; }  // UTC

        [Timestamp] // Optimistic concurrency token to handle concurrent updates
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(ResourceId))]
        public Resource? Resource { get; set; }
    }
}
