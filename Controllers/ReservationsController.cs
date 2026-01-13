using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarReservationSystemApp;

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
            // Pobierz wszystkie rezerwacje z załadowanymi samochodami
            var reservations = await _context.Reservations
                .Include(r => r.Car)
                .OrderByDescending(r => r.From)
                .ThenByDescending(r => r.Id)
                .ToListAsync();

            // Debug: sprawdź czy są rezerwacje
            var count = reservations.Count;
            if (count > 0)
            {
                TempData["InfoMessage"] = $"Znaleziono {count} rezerwacji.";
            }

            return View(reservations);
        }

        // FORMULARZ
        public IActionResult Create(int? carId = null)
        {
            ViewBag.Cars = _context.Cars
                .Where(c => c.IsAvailable)
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

            if (carId.HasValue)
            {
                var car = _context.Cars.Find(carId.Value);
                if (car != null)
                {
                    ViewBag.SelectedCarPrice = CarsController.GetPricePerDay(car);
                }
            }

            return View();
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

            // Przywrócenie listy samochodów w przypadku błędu
            ViewBag.Cars = _context.Cars
                .Where(c => c.IsAvailable)
                .Select(c => new
                {
                    c.Id,
                    Display = c.Brand + " " + c.Model
                })
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Display,
                    Selected = c.Id == reservation.CarId
                })
                .ToList();

            if (reservation.CarId > 0)
            {
                var selectedCar = await _context.Cars.FindAsync(reservation.CarId);
                if (selectedCar != null)
                {
                    ViewBag.SelectedCarPrice = CarsController.GetPricePerDay(selectedCar);
                }
            }

            return View(reservation);
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
    }
}
