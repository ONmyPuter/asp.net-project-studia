using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CarReservationSystemApp
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public DateTime From { get; set; }

        [Required]
        public DateTime To { get; set; }

        public int CarId { get; set; }
        public Car? Car { get; set; }

        // Link to the user who made this reservation
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}
