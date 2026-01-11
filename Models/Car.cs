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

        public bool IsAvailable { get; set; } = true;

        public ICollection<Reservation> Reservations { get; set; }
    }
}
