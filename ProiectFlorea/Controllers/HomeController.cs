using Microsoft.AspNetCore.Mvc;
using ProiectFlorea.Data;
using ProiectFlorea.Models;
using System.Diagnostics;

namespace ProiectFlorea.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var readings = _context.TemperatureReadings
                .OrderBy(x => x.CreatedAt)
                .ToList();

            return View(readings);
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
}