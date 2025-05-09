using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.DbContexts
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options) : base(options)
        {
        }

        // add DbSet properties
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservedRoom> ReservedRooms { get; set; }
        public DbSet<ReservedPackage> ReservedPackages { get; set; }
        public DbSet<Pricing> Pricings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; } 
        public DbSet<User> Users { get; set; }
        public DbSet<FacilityType> FacilityTypes { get; set; }
        public DbSet<RoomPricing> RoomPricings { get; set; }
        public DbSet<Image> Images { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
