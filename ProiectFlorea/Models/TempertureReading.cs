using System.ComponentModel.DataAnnotations;

namespace ProiectFlorea.Models
{
    public class TemperatureReading
    {
        public int Id { get; set; }

        [Required]
        public double TemperatureC { get; set; }

        [Required]
        public int VoltageMv { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}