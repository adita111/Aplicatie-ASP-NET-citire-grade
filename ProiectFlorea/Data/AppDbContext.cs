using Microsoft.EntityFrameworkCore;

namespace ProiectFlorea.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
