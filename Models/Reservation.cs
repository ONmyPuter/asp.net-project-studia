using System.ComponentModel.DataAnnotations;

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
        public Car Car { get; set; }
    }
}
