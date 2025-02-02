using Microsoft.EntityFrameworkCore;
using AirportDemo.Models;

namespace AirportDemo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } // Kullanıcı tablosu tanımlandı
        public DbSet<Flight> Flights { get; set; } // Mevcut flight tablosu
    }
}
