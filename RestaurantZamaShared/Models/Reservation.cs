using System.ComponentModel.DataAnnotations;

namespace RestaurantZamaShared.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        public TimeSpan ReservationTime { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }

        [Required]
        [MaxLength(20)]
        public string ContactPhone { get; set; }

        [MaxLength(500)]
        public string? SpecialRequests { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}