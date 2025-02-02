using Microsoft.EntityFrameworkCore;
using AirportDemo.Models;

namespace AirportDemo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Flight> Flights { get; set; }
    }
}
