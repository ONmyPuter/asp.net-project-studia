using System.ComponentModel.DataAnnotations;

namespace CarReservationSystemApp
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        public int Seats { get; set; }

        public string FuelType { get; set; } = string.Empty;

        public string Transmission { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public ICollection<Reservation> Reservations { get; set; }
    }
}
