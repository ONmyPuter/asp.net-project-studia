using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarReservationSystemApp
{
    [ApiController]
    [Route("api/cars")]
    public class CarsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/cars
        // Zwraca wszystkie dostępne auta
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cars = await _context.Cars
                .Where(c => c.IsAvailable)
                .Select(c => new
                {
                    c.Id,
                    c.Brand,
                    c.Model,
                    c.Seats
                })
                .ToListAsync();

            return Ok(cars);
        }

        // GET: api/cars/5
        // Szczegóły jednego auta
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var car = await _context.Cars
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Brand,
                    c.Model,
                    c.Seats,
                    c.IsAvailable
                })
                .FirstOrDefaultAsync();

            if (car == null)
                return NotFound();

            return Ok(car);
        }

        // POST: api/cars
        // Dodawanie auta (admin)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = car.Id },
                car
            );
        }

        // PUT: api/cars/5
        // Aktualizacja auta
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Car updatedCar)
        {
            if (id != updatedCar.Id)
                return BadRequest("Id w URL i obiekcie nie są zgodne.");

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound();

            car.Brand = updatedCar.Brand;
            car.Model = updatedCar.Model;
            car.Seats = updatedCar.Seats;
            car.IsAvailable = updatedCar.IsAvailable;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/cars/5
        // Usuwanie auta
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
