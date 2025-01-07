using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantZamaApp.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }  // Făcut opțional prin adăugarea ?

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        public int LoyaltyPoints { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}