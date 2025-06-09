using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarSharingApp.Data;
using CarSharingApp.Models;

namespace CarSharingApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);

            if (user == null)
            {
                return Challenge();
            }

            // Get active booking
            var activeBooking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.UserId == user.Id && b.Status == BookingStatus.Active);

            // Get recent bookings
            var recentBookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.CreatedDate)
                .Take(5)
                .ToListAsync();

            // Get nearby available cars
            var nearbyCars = await _context.Cars
                .Where(c => c.Status == CarStatus.Available)
                .OrderBy(c => c.Brand)
                .Take(6)
                .ToListAsync();

            // Calculate statistics
            var totalTrips = await _context.Bookings
                .CountAsync(b => b.UserId == user.Id && b.Status == BookingStatus.Completed);

            var totalCost = await _context.Bookings
                .Where(b => b.UserId == user.Id && b.Status == BookingStatus.Completed)
                .SumAsync(b => b.TotalCost ?? 0);

            var totalTime = await _context.Bookings
                .Where(b => b.UserId == user.Id && b.Status == BookingStatus.Completed)
                .SumAsync(b => b.ActualDurationMinutes ?? 0);

            ViewBag.ActiveBooking = activeBooking;
            ViewBag.RecentBookings = recentBookings;
            ViewBag.NearbyCars = nearbyCars;
            ViewBag.TotalTrips = totalTrips;
            ViewBag.TotalCost = totalCost;
            ViewBag.TotalTimeHours = Math.Round(totalTime / 60.0, 1);
            ViewBag.User = user;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndTrip(int bookingId, int endMileage, int endFuelLevel)
        {
            var userId = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == user!.Id && b.Status == BookingStatus.Active);

            if (booking == null)
            {
                TempData["Error"] = "Бронирование не найдено.";
                return RedirectToAction(nameof(Index));
            }

            // Calculate trip details
            booking.EndTime = DateTime.Now;
            booking.ActualDurationMinutes = (booking.EndTime.HasValue && booking.StartTime.HasValue) ?
                             (int)((booking.EndTime.Value - booking.StartTime.Value).TotalMinutes) : 0;
            booking.EndMileage = endMileage;
            booking.EndFuelLevel = endFuelLevel;
            booking.TotalCost = booking.ActualDurationMinutes * booking.Car.PricePerMinute;
            booking.Status = BookingStatus.Completed;

            // Update car status and details
            booking.Car.Status = CarStatus.Available;
            booking.Car.Mileage = endMileage;
            booking.Car.FuelLevel = endFuelLevel;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Поездка завершена. Стоимость: {booking.TotalCost:C}";
            return RedirectToAction(nameof(Index));
        }
    }
}