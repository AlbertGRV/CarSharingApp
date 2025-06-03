using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarSharingApp.Data;
using CarSharingApp.Models;

namespace CarSharingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalCars = await _context.Cars.CountAsync(),
                AvailableCars = await _context.Cars.CountAsync(c => c.Status == CarStatus.Available),
                ActiveBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Active),
                TodayRevenue = await _context.Bookings
                    .Where(b => b.EndTime.HasValue && b.EndTime.Value.Date == DateTime.Today)
                    .SumAsync(b => b.TotalCost ?? 0),
                TotalRevenue = await _context.Bookings
                    .Where(b => b.Status == BookingStatus.Completed)
                    .SumAsync(b => b.TotalCost ?? 0)
            };

            var recentBookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Car)
                .OrderByDescending(b => b.CreatedDate)
                .Take(10)
                .ToListAsync();

            ViewBag.Stats = stats;
            ViewBag.RecentBookings = recentBookings;

            return View();
        }

        // Cars Management
        public async Task<IActionResult> Cars()
        {
            var cars = await _context.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Model).ToListAsync();
            return View(cars);
        }

        public IActionResult CreateCar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCar(Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Cars.Add(car);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Автомобиль успешно добавлен.";
                return RedirectToAction(nameof(Cars));
            }
            return View(car);
        }

        public async Task<IActionResult> EditCar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(int id, Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Автомобиль успешно обновлен.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Cars));
            }
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                // Check if car has active bookings
                var hasActiveBookings = await _context.Bookings
                    .AnyAsync(b => b.CarId == id && b.Status == BookingStatus.Active);

                if (hasActiveBookings)
                {
                    TempData["Error"] = "Нельзя удалить автомобиль с активными бронированиями.";
                }
                else
                {
                    _context.Cars.Remove(car);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Автомобиль успешно удален.";
                }
            }
            return RedirectToAction(nameof(Cars));
        }

        // Users Management
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
            return View(users);
        }

        // Bookings Management
        public async Task<IActionResult> Bookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Car)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
            return View(bookings);
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}