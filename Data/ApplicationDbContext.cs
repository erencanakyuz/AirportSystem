using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AirportDemo.Models;

namespace AirportDemo.Data
{
    // **Identity için User ve Role'u int olarak belirttik**
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<FuelManagement> FuelManagements { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Logistics> Logistics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity için gerekli yapılandırmaları uygula

            // **Identity tablolarını güncelliyoruz**
            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole<int>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        }
    }
}
