using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarSharingApp.Data;
using CarSharingApp.Models;
using System.Diagnostics;

namespace CarSharingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var availableCars = await _context.Cars
                .Where(c => c.Status == CarStatus.Available)
                .Take(6)
                .ToListAsync();

            ViewBag.AvailableCarsCount = await _context.Cars
                .CountAsync(c => c.Status == CarStatus.Available);

            ViewBag.TotalUsers = await _context.Users.CountAsync();

            return View(availableCars);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}