using System.ComponentModel.DataAnnotations;

namespace CarReservationSystemApp
{
    public class Location
    {
        public int Id { get; set; }

        [Required]
        public string City { get; set; }

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
