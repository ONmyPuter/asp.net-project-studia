using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CarReservationSystemApp;

namespace CarReservationSystemApp.Controllers
{
    [Authorize]
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
            var query = _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .Include(r => r.PickupLocation)
                .Include(r => r.DropoffLocation)
                .Include(r => r.InsurancePolicy)
                .OrderByDescending(r => r.From)
                .ThenByDescending(r => r.Id);

            IQueryable<Reservation> filteredQuery;
            
            // Admin sees all reservations, regular users see only their own
            if (User.IsInRole("Admin"))
            {
                filteredQuery = query;
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                filteredQuery = query.Where(r => r.UserId == userId);
            }

            var reservations = await filteredQuery.ToListAsync();

            // Debug: sprawdź czy są rezerwacje
            var count = reservations.Count;
            if (count > 0)
            {
                TempData["InfoMessage"] = $"Znaleziono {count} rezerwacji.";
            }

            return View(reservations);
        }

        // FORMULARZ
        public IActionResult Create(int? carId = null, int? pickupLocationId = null)
        {
            PopulateViewBagForCreate(carId, pickupLocationId);

            var reservation = new Reservation 
            { 
                CarId = carId ?? 0,
                PickupLocationId = pickupLocationId ?? 0,
                DropoffLocationId = pickupLocationId ?? 0, // Default to same location
                InsurancePolicyId = 1, // Default to Basic
                From = DateTime.Today,
                To = DateTime.Today.AddDays(1)
            };

            return View(reservation);
        }

        // API endpoint to get cars by location (for AJAX)
        [HttpGet]
        public JsonResult GetCarsByLocation(int locationId)
        {
            var cars = _context.Cars
                .Where(c => c.IsAvailable && c.CurrentLocationId == locationId)
                .Select(c => new
                {
                    id = c.Id,
                    text = c.Brand + " " + c.Model,
                    price = CarsController.GetPricePerDay(c)
                })
                .ToList();

            return Json(cars);
        }

        // ZAPIS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            // Normalizacja dat - ustawienie czasu na początek dnia
            if (reservation.From != default(DateTime))
            {
                reservation.From = reservation.From.Date;
            }
            if (reservation.To != default(DateTime))
            {
                reservation.To = reservation.To.Date;
            }

            // Walidacja dat
            if (reservation.From >= reservation.To)
            {
                ModelState.AddModelError("To", "Data zakończenia musi być późniejsza niż data rozpoczęcia.");
            }

            if (reservation.From < DateTime.Today)
            {
                ModelState.AddModelError("From", "Data rozpoczęcia nie może być w przeszłości.");
            }

            // Walidacja lokalizacji
            if (reservation.PickupLocationId <= 0)
            {
                ModelState.AddModelError("PickupLocationId", "Wybierz lokalizację odbioru.");
            }

            if (reservation.DropoffLocationId <= 0)
            {
                ModelState.AddModelError("DropoffLocationId", "Wybierz lokalizację zwrotu.");
            }

            // Walidacja ubezpieczenia
            if (reservation.InsurancePolicyId <= 0)
            {
                ModelState.AddModelError("InsurancePolicyId", "Wybierz ubezpieczenie.");
            }

            // Sprawdzenie czy samochód istnieje i jest dostępny
            var car = await _context.Cars.FindAsync(reservation.CarId);
            if (car == null)
            {
                ModelState.AddModelError("CarId", "Wybrany samochód nie istnieje.");
            }
            else if (!car.IsAvailable)
            {
                ModelState.AddModelError("CarId", "Wybrany samochód jest niedostępny.");
            }
            else if (car.CurrentLocationId != reservation.PickupLocationId)
            {
                ModelState.AddModelError("CarId", "Wybrany samochód nie jest dostępny w wybranej lokalizacji odbioru.");
            }
            else
            {
                // Sprawdzenie dostępności w wybranym terminie
                if (!IsCarAvailable(reservation.CarId, reservation.From, reservation.To))
                {
                    ModelState.AddModelError("", "Samochód jest już zarezerwowany w wybranym terminie. Wybierz inny termin lub samochód.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Set the current user as the owner of this reservation
                    reservation.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    
                    _context.Reservations.Add(reservation);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Rezerwacja została utworzona pomyślnie! (ID: {reservation.Id})";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Błąd podczas zapisywania rezerwacji: {ex.Message}");
                }
            }

            // Przywrócenie danych w przypadku błędu
            PopulateViewBagForCreate(reservation.CarId, reservation.PickupLocationId);

            return View(reservation);
        }

        // Helper method to populate ViewBag for Create view
        private void PopulateViewBagForCreate(int? carId, int? pickupLocationId)
        {
            // Locations
            ViewBag.Locations = new SelectList(_context.Locations, "Id", "City", pickupLocationId);

            // Insurance policies
            ViewBag.InsurancePolicies = _context.InsurancePolicies
                .Where(i => i.IsActive)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.PolicyType} - {i.PricePerDay:C}/dzień"
                })
                .ToList();

            // Cars - filtered by location if specified
            IQueryable<Car> carsQuery = _context.Cars.Where(c => c.IsAvailable);
            
            if (pickupLocationId.HasValue && pickupLocationId.Value > 0)
            {
                carsQuery = carsQuery.Where(c => c.CurrentLocationId == pickupLocationId.Value);
            }

            ViewBag.Cars = carsQuery
                .Select(c => new
                {
                    c.Id,
                    Display = c.Brand + " " + c.Model
                })
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Display,
                    Selected = carId.HasValue && c.Id == carId.Value
                })
                .ToList();

            if (carId.HasValue && carId.Value > 0)
            {
                var car = _context.Cars.Find(carId.Value);
                if (car != null)
                {
                    ViewBag.SelectedCarPrice = CarsController.GetPricePerDay(car);
                }
            }
        }

        // Metoda sprawdzająca dostępność samochodu w danym terminie
        private bool IsCarAvailable(int carId, DateTime from, DateTime to)
        {
            // Sprawdzenie czy istnieją rezerwacje, które kolidują z wybranym terminem
            var conflictingReservations = _context.Reservations
                .Where(r => r.CarId == carId)
                .Where(r => 
                    (r.From <= from && r.To > from) ||  // Rezerwacja zaczyna się przed i kończy po rozpoczęciu
                    (r.From < to && r.To >= to) ||      // Rezerwacja zaczyna się przed końcem i kończy po zakończeniu
                    (r.From >= from && r.To <= to)      // Rezerwacja jest całkowicie w wybranym terminie
                )
                .Any();

            return !conflictingReservations;
        }

        // SZCZEGÓŁY REZERWACJI - Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Car)
                    .ThenInclude(c => c.CurrentLocation)
                .Include(r => r.User)
                .Include(r => r.PickupLocation)
                .Include(r => r.DropoffLocation)
                .Include(r => r.InsurancePolicy)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // USUNIĘCIE REZERWACJI - Admin only (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .Include(r => r.PickupLocation)
                .Include(r => r.DropoffLocation)
                .Include(r => r.InsurancePolicy)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // OZNACZENIE REZERWACJI JAKO ZAKOŃCZONEJ - Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsFinished(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.IsFinished = true;
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = $"Rezerwacja #{id} została oznaczona jako zakończona.";
            return RedirectToAction(nameof(Index));
        }

        // USUNIĘCIE REZERWACJI - Admin only (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Rezerwacja #{id} została pomyślnie usunięta.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

