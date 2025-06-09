using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarSharingApp.Data;
using CarSharingApp.Models;
using System.Threading.Tasks;
using System.Linq;

namespace CarSharingApp.Controllers
{
    [Authorize]
    public class UserDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserDashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Главная страница личного кабинета
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Обновляем время последнего входа
            user.LastLoginAt = DateTime.Now;
            await _userManager.UpdateAsync(user);

            // Получаем статистику пользователя
            var activeBookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == user.Id && b.Status == BookingStatus.Active)
                .OrderBy(b => b.StartTime)
                .ToListAsync();

            var recentBookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToListAsync();

            var totalBookings = await _context.Bookings
                .CountAsync(b => b.UserId == user.Id);

            var totalSpent = await _context.Bookings
                .Where(b => b.UserId == user.Id && b.Status == BookingStatus.Completed)
                .SumAsync(b => b.TotalCost);

            ViewBag.User = user;
            ViewBag.ActiveBookings = activeBookings;
            ViewBag.RecentBookings = recentBookings;
            ViewBag.TotalBookings = totalBookings;
            ViewBag.TotalSpent = totalSpent;

            return View();
        }

        // Страница профиля
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Обновление профиля
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Обновляем только разрешенные поля
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.City = model.City;
            user.PostalCode = model.PostalCode;
            user.Country = model.Country;
            user.DateOfBirth = model.DateOfBirth;
            user.DriverLicenseNumber = model.DriverLicenseNumber;
            user.DriverLicenseIssueDate = model.DriverLicenseIssueDate;
            user.DriverLicenseExpiryDate = model.DriverLicenseExpiryDate;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Профиль успешно обновлен";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка при обновлении профиля";
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction(nameof(Profile));
        }

        // Страница бронирований
        public async Task<IActionResult> Bookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bookings);
        }

        // Детали бронирования
        public async Task<IActionResult> BookingDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == user.Id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // Отмена бронирования
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == user.Id);

            if (booking == null)
            {
                return NotFound();
            }

            // Можно отменить только будущие бронирования
            if (booking.StartTime <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Нельзя отменить начавшееся бронирование";
                return RedirectToAction(nameof(BookingDetails), new { id });
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = "Отменено пользователем";

            // Освобождаем автомобиль
            var car = await _context.Cars.FindAsync(booking.CarId);
            if (car != null)
            {
                car.Status = CarStatus.Available;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Бронирование успешно отменено";
            return RedirectToAction(nameof(Bookings));
        }

        // Быстрое бронирование
        public async Task<IActionResult> QuickBooking()
        {
            var availableCars = await _context.Cars
                .Where(c => c.Status == CarStatus.Available)
                .OrderBy(c => c.PricePerMinute)
                .ToListAsync();

            return View(availableCars);
        }

        // Уведомления
        public async Task<IActionResult> Notifications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Создаем тестовые уведомления (в реальном проекте они будут из базы)
            var notifications = new List<dynamic>
            {
                new {
                    Title = "Добро пожаловать!",
                    Message = "Спасибо за регистрацию в CarShare",
                    Date = DateTime.Now.AddDays(-1),
                    Type = "info",
                    IsRead = false
                },
                new {
                    Title = "Скидка 20%",
                    Message = "Специальное предложение на выходные",
                    Date = DateTime.Now.AddHours(-2),
                    Type = "promotion",
                    IsRead = false
                }
            };

            return View(notifications);
        }

        // Настройки
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}