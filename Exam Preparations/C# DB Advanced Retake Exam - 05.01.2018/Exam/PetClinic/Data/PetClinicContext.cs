namespace PetClinic.Data
{
    using Microsoft.EntityFrameworkCore;
    using PetClinic.Models;

    public class PetClinicContext : DbContext
    {
        public PetClinicContext() { }

        public PetClinicContext(DbContextOptions options)
            :base(options) { }

        public DbSet<Animal> Animals { get; set; }

        public DbSet<AnimalAid> AnimalAids { get; set; }

        public DbSet<Passport> Passports { get; set; }

        public DbSet<Procedure> Procedures { get; set; }

        public DbSet<ProcedureAnimalAid> ProceduresAnimalAids { get; set; }

        public DbSet<Vet> Vets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProcedureAnimalAid>(e =>
                e.HasKey(a => new { a.ProcedureId, a.AnimalAidId }));

            builder.Entity<Procedure>(e =>
                e.HasMany(a => a.ProcedureAnimalAids)
                .WithOne(b => b.Procedure)
                .HasForeignKey(b => b.ProcedureId));

            builder.Entity<AnimalAid>(e =>
                e.HasMany(a => a.AnimalAidProcedures)
                .WithOne(b => b.AnimalAid)
                .HasForeignKey(b => b.AnimalAidId));

            builder.Entity<AnimalAid>(e =>
                e.HasAlternateKey(a => a.Name));

            builder.Entity<Vet>(e =>
                e.HasAlternateKey(a => a.PhoneNumber));
        }
    }
}
