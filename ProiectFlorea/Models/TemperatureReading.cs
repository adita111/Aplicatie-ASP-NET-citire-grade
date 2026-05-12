using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProiectFlorea.Models
{
    public class TemperatureReading
    {
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("temperature_c")]
        public double? TemperatureC { get; set; }

        [Required]
        [JsonPropertyName("voltage_mv")]
        public int? VoltageMv { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}