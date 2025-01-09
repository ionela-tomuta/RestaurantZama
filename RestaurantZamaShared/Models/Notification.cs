using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantZamaShared.Models
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Type { get; set; }

        public bool IsRead { get; set; }

        [NotMapped]
        public User User { get; set; }
    }
}