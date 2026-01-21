using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarReservationSystemApp
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<InsurancePolicy> InsurancePolicies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Car -> Location relationship
            modelBuilder.Entity<Car>()
                .HasOne(c => c.CurrentLocation)
                .WithMany(l => l.Cars)
                .HasForeignKey(c => c.CurrentLocationId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Reservation -> PickupLocation relationship
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.PickupLocation)
                .WithMany()
                .HasForeignKey(r => r.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Reservation -> DropoffLocation relationship
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.DropoffLocation)
                .WithMany()
                .HasForeignKey(r => r.DropoffLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Reservation -> InsurancePolicy relationship
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.InsurancePolicy)
                .WithMany(i => i.Reservations)
                .HasForeignKey(r => r.InsurancePolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed 3 hardcoded insurance policies
            modelBuilder.Entity<InsurancePolicy>().HasData(
                new InsurancePolicy
                {
                    Id = 1,
                    PolicyNumber = "INS-BASIC-001",
                    PolicyType = "Basic",
                    Description = "Podstawowe ubezpieczenie OC. Pokrywa szkody wyrządzone osobom trzecim.",
                    PricePerDay = 15.00m,
                    IsActive = true
                },
                new InsurancePolicy
                {
                    Id = 2,
                    PolicyNumber = "INS-STANDARD-001",
                    PolicyType = "Standard",
                    Description = "Standardowe ubezpieczenie OC + AC. Pokrywa szkody własne i osobom trzecim.",
                    PricePerDay = 35.00m,
                    IsActive = true
                },
                new InsurancePolicy
                {
                    Id = 3,
                    PolicyNumber = "INS-PREMIUM-001",
                    PolicyType = "Premium",
                    Description = "Pełne ubezpieczenie OC + AC + NNW. Maksymalna ochrona bez udziału własnego.",
                    PricePerDay = 60.00m,
                    IsActive = true
                }
            );
        }
    }
}
