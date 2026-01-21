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

        // Pickup and dropoff locations
        public int PickupLocationId { get; set; }
        public Location? PickupLocation { get; set; }

        public int DropoffLocationId { get; set; }
        public Location? DropoffLocation { get; set; }

        // Insurance policy for this reservation
        public int InsurancePolicyId { get; set; }
        public InsurancePolicy? InsurancePolicy { get; set; }

        public bool IsFinished { get; set; } = false;
    }
}
