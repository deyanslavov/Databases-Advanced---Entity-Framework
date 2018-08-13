namespace SoftJail.Data
{
    using Microsoft.EntityFrameworkCore;
    using SoftJail.Data.Models;

    public class SoftJailDbContext : DbContext
    {
        public SoftJailDbContext()
        {
        }

        public SoftJailDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Cell> Cells { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Mail> Mails { get; set; }

        public DbSet<Officer> Officers { get; set; }

        public DbSet<OfficerPrisoner> OfficersPrisoners { get; set; }

        public DbSet<Prisoner> Prisoners { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<OfficerPrisoner>().HasKey(x => new { x.OfficerId, x.PrisonerId });

            builder.Entity<Officer>().HasMany(e => e.OfficerPrisoners)
                                     .WithOne(a => a.Officer)
                                     .HasForeignKey(a => a.OfficerId)
                                      .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Prisoner>().HasMany(e => e.PrisonerOfficers)
                                      .WithOne(a => a.Prisoner)
                                      .HasForeignKey(a => a.PrisonerId)
                                      .OnDelete(DeleteBehavior.Restrict);
        }
    }
}