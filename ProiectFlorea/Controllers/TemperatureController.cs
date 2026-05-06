using Microsoft.AspNetCore.Mvc;
using ProiectFlorea.Data;
using ProiectFlorea.Models;

namespace ProiectFlorea.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemperatureController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TemperatureController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult PostTemperature([FromBody] TemperatureReading data)
        {
            if (data == null)
                return BadRequest();

            _context.TemperatureReadings.Add(data);
            _context.SaveChanges();

            return Ok(new { message = "Saved" });
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.TemperatureReadings.ToList());
        }
    }
}