using Microsoft.EntityFrameworkCore;
using ProiectFlorea.Models;

namespace ProiectFlorea.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TemperatureReading> TemperatureReadings { get; set; }
    }
}