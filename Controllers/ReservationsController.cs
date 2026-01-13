using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarReservationSystemApp.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTA
        public async Task<IActionResult> Index()
        {
            var reservations = _context.Reservations
                .Include(r => r.Car);

            return View(await reservations.ToListAsync());
        }

        // FORMULARZ
        public IActionResult Create()
        {
           ViewBag.Cars = _context.Cars
    .Select(c => new
    {
        c.Id,
        Display = c.Brand + " " + c.Model
    })
    .Select(c => new SelectListItem
    {
        Value = c.Id.ToString(),
        Text = c.Display
    })
    .ToList();

            return View();
        }

        // ZAPIS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Cars = new SelectList(_context.Cars, "Id", "Brand");
            return View(reservation);
        }
    }
}
