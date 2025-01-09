using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace RestaurantZamaShared.Models
{
    public class Table
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Number { get; set; }

        [Required]
        public int Capacity { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Required]
        [MaxLength(50)]
        public string Location { get; set; }

        [NotMapped]
        public List<string> Features { get; set; } = new();

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}