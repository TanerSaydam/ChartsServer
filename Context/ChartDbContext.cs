using ChartsServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ChartsServer.Context
{
    public class ChartDbContext : DbContext
    {
        public ChartDbContext(DbContextOptions options) : base(options)
        {
        }

        public ChartDbContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=SatisDb;Integrated Security=true");
        }

        public DbSet<Personel> Personeller { get; set; }
        public DbSet<Satis> Satislar { get; set; }
    }
}
