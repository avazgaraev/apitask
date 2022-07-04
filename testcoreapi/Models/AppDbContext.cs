using Microsoft.EntityFrameworkCore;

namespace testcoreapi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<WeatherForecast> WeatherForecasts{ get; set; }
    }
}
