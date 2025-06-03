using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarSharingApp.Data;
using CarSharingApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace CarSharingApp.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string? search, FuelType? fuelType, TransmissionType? transmission, string? sortBy)
        {
            var carsQuery = _context.Cars.Where(c => c.Status == CarStatus.Available);

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                carsQuery = carsQuery.Where(c =>
                    c.Brand.Contains(search) ||
                    c.Model.Contains(search) ||
                    c.Color.Contains(search));
            }

            // Fuel type filter
            if (fuelType.HasValue)
            {
                carsQuery = carsQuery.Where(c => c.FuelType == fuelType.Value);
            }

            // Transmission filter
            if (transmission.HasValue)
            {
                carsQuery = carsQuery.Where(c => c.TransmissionType == transmission.Value);
            }

            // Sorting
            carsQuery = sortBy switch
            {
                "price_asc" => carsQuery.OrderBy(c => c.PricePerMinute),
                "price_desc" => carsQuery.OrderByDescending(c => c.PricePerMinute),
                "year_desc" => carsQuery.OrderByDescending(c => c.Year),
                "brand" => carsQuery.OrderBy(c => c.Brand).ThenBy(c => c.Model),
                _ => carsQuery.OrderBy(c => c.Brand).ThenBy(c => c.Model)
            };

            ViewBag.Search = search;
            ViewBag.FuelType = fuelType;
            ViewBag.Transmission = transmission;
            ViewBag.SortBy = sortBy;

            return View(await carsQuery.ToListAsync());
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Book/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int id, int plannedDuration)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null || car.Status != CarStatus.Available)
            {
                TempData["Error"] = "Автомобиль недоступен для бронирования.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var userId = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);

            if (user == null)
            {
                return Challenge();
            }

            // Check if user has active bookings
            var activeBooking = await _context.Bookings
                .AnyAsync(b => b.UserId == user.Id && b.Status == BookingStatus.Active);

            if (activeBooking)
            {
                TempData["Error"] = "У вас уже есть активное бронирование.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var booking = new Booking
            {
                UserId = user.Id,
                CarId = car.Id,
                StartTime = DateTime.Now,
                PlannedDurationMinutes = plannedDuration,
                StartMileage = car.Mileage,
                StartFuelLevel = car.FuelLevel,
                Status = BookingStatus.Active
            };

            car.Status = CarStatus.InUse;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Автомобиль успешно забронирован!";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}