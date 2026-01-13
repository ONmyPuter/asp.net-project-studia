using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarReservationSystemApp
{
    [Route("Cars")]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cars
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars.ToListAsync();
            return View(cars);
        }

        // GET: /Cars/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Cars/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Cars.Add(car);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Samochód {car.Brand} {car.Model} został dodany pomyślnie!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Wystąpiły błędy podczas dodawania samochodu. Sprawdź wprowadzone dane.";
            return View(car);
        }

        // Metoda pomocnicza do obliczania ceny za dobę
        public static decimal GetPricePerDay(Car car)
        {
            // Ceny na podstawie marki i modelu
            var brand = car.Brand?.ToLower() ?? "";
            var model = car.Model?.ToLower() ?? "";

            // Luksusowe marki
            if (brand.Contains("bmw") || brand.Contains("mercedes") || brand.Contains("audi") || 
                brand.Contains("porsche") || brand.Contains("lexus"))
            {
                return 250.00m;
            }

            // Marki premium
            if (brand.Contains("volvo") || brand.Contains("volkswagen") || brand.Contains("skoda") ||
                brand.Contains("seat") || brand.Contains("opel"))
            {
                return 180.00m;
            }

            // Marki ekonomiczne
            if (brand.Contains("toyota") || brand.Contains("honda") || brand.Contains("mazda") ||
                brand.Contains("ford") || brand.Contains("peugeot") || brand.Contains("renault"))
            {
                return 150.00m;
            }

            // Domyślna cena
            return 120.00m;
        }

        // Metoda pomocnicza do generowania opisu samochodu
        public static string GetCarDescription(Car car)
        {
            var brand = car.Brand?.ToLower() ?? "";
            var model = car.Model?.ToLower() ?? "";
            var seats = car.Seats;

            var description = "";

            // Opis na podstawie liczby miejsc
            if (seats <= 2)
            {
                description = "Kompaktowy samochód miejski, idealny do poruszania się po mieście.";
            }
            else if (seats <= 5)
            {
                description = "Samochód osobowy, komfortowy dla całej rodziny. Doskonały do codziennych podróży.";
            }
            else if (seats <= 7)
            {
                description = "Pojazd 7-osobowy, idealny dla większych rodzin. Przestronny bagażnik i wygodne wnętrze.";
            }
            else
            {
                description = "Duży pojazd, idealny do przewozu większej liczby pasażerów.";
            }

            // Dodatkowe informacje na podstawie marki
            if (brand.Contains("bmw") || brand.Contains("mercedes") || brand.Contains("audi"))
            {
                description += " Luksusowe wyposażenie i najwyższa jakość wykonania.";
            }
            else if (brand.Contains("toyota") || brand.Contains("honda"))
            {
                description += " Sprawdzona niezawodność i niskie koszty eksploatacji.";
            }

            return description;
        }
    }
}
