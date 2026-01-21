using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarReservationSystemApp
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reservations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .Select(r => new
                {
                    r.Id,
                    r.From,
                    r.To,
                    Car = r.Car == null ? null : new { r.Car.Id, r.Car.Brand, r.Car.Model },
                    User = r.User == null ? null : new { r.User.Id, r.User.UserName, r.User.Email },
                    r.PickupLocationId,
                    r.DropoffLocationId
                })
                .ToListAsync();

            return Ok(reservations);
        }

        // GET: api/reservations/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .Where(r => r.Id == id)
                .Select(r => new
                {
                    r.Id,
                    r.From,
                    r.To,
                    Car = r.Car == null ? null : new { r.Car.Id, r.Car.Brand, r.Car.Model },
                    User = r.User == null ? null : new { r.User.Id, r.User.UserName, r.User.Email },
                    r.PickupLocationId,
                    r.DropoffLocationId
                })
                .FirstOrDefaultAsync();

            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Reservation reservation)
        {
            // Clear navigation properties to avoid validation errors if they are not fully populated or null
            // We expect the user to send IDs (CarId, UserId, etc.)
            ModelState.Remove("Car");
            ModelState.Remove("User");
            ModelState.Remove("PickupLocation");
            ModelState.Remove("DropoffLocation");
            ModelState.Remove("InsurancePolicy");

            // For experimentation, if UserId is not provided, we might fail or allow null depending on model.
            // Model says: public string? UserId { get; set; } so it's nullable.

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = reservation.Id },
                reservation
            );
        }

        // PUT: api/reservations/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Reservation updatedReservation)
        {
            if (id != updatedReservation.Id)
                return BadRequest("Id in URL and body do not match.");

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            // Update fields
            reservation.From = updatedReservation.From;
            reservation.To = updatedReservation.To;
            reservation.CarId = updatedReservation.CarId;
            reservation.UserId = updatedReservation.UserId;
            reservation.PickupLocationId = updatedReservation.PickupLocationId;
            reservation.DropoffLocationId = updatedReservation.DropoffLocationId;
            reservation.InsurancePolicyId = updatedReservation.InsurancePolicyId;

            // Simple update ignoring navigation properties logic for experimentation

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/reservations/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
