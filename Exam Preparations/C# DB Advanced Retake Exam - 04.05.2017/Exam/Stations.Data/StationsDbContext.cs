namespace Stations.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;

    public class StationsDbContext : DbContext
    {
        public StationsDbContext() { }

        public StationsDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<CustomerCard> Cards { get; set; }

        public DbSet<SeatingClass> SeatingClasses { get; set; }

        public DbSet<Station> Stations { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Train> Trains { get; set; }

        public DbSet<TrainSeat> TrainSeats { get; set; }

        public DbSet<Trip> Trips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Station>(entity =>
            {
                entity.HasAlternateKey(e => e.Name);

                entity.HasMany(e => e.TripsFrom)
                      .WithOne(e => e.OriginStation)
                      .HasForeignKey(e => e.OriginStationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.TripsTo)
                      .WithOne(e => e.DestinationStation)
                      .HasForeignKey(e => e.DestinationStationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Train>(entity =>
            {
                entity.HasAlternateKey(e => e.TrainNumber);

                entity.HasMany(e => e.TrainSeats)
                      .WithOne(e => e.Train)
                      .HasForeignKey(e => e.TrainId);

                entity.HasMany(e => e.Trips)
                      .WithOne(e => e.Train)
                      .HasForeignKey(e => e.TrainId);
            });

            modelBuilder.Entity<SeatingClass>(entity =>
            {
                entity.HasAlternateKey(e => e.Name);

                entity.HasAlternateKey(e => e.Abbreviation);
            });

            modelBuilder.Entity<CustomerCard>(entity =>
            {
                entity.HasMany(e => e.BoughtTickets)
                      .WithOne(e => e.CustomerCard)
                      .HasForeignKey(e => e.CustomerCardId);
            });
        }
    }
}