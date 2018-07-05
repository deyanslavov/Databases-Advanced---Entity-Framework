namespace AutoMapper.Data
{
    using Microsoft.EntityFrameworkCore;

    using AutoMapper.Models;
    using AutoMapper.Data.ModelConfigurations;

    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext() { }

        public EmployeeDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        }
    }
}
