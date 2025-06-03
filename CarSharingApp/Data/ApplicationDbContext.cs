using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarSharingApp.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CarSharingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Car configuration
            builder.Entity<Car>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Brand).HasMaxLength(50);
                entity.Property(e => e.Model).HasMaxLength(50);
                entity.Property(e => e.Color).HasMaxLength(30);
                entity.Property(e => e.LicensePlate).HasMaxLength(20);
                entity.Property(e => e.VinNumber).HasMaxLength(17);
                entity.Property(e => e.PricePerMinute).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            // Booking configuration
            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ApplicationUser configuration
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.DriverLicenseNumber).HasMaxLength(20);
            });
        }
    }
}