using System.ComponentModel.DataAnnotations;

namespace CarReservationSystemApp
{
    public class InsurancePolicy
    {
        public int Id { get; set; }

        [Required]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required]
        public string PolicyType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal PricePerDay { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
