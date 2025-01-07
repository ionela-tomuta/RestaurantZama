using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantZamaApp.Models
{
    [Table("Reservations")]
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int TableId { get; set; }

        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        public TimeSpan ReservationTime { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [MaxLength(500)]
        public string? SpecialRequests { get; set; }

        [Required]
        [Phone]
        public string ContactPhone { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("TableId")]
        public virtual Table? Table { get; set; }
    }
}