namespace FastFood.Data
{
    using FastFood.Models;
    using Microsoft.EntityFrameworkCore;

    public class FastFoodDbContext : DbContext
    {
        public FastFoodDbContext()
        {
        }

        public FastFoodDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Position> Positions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                builder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Employee>(entity =>
                entity.HasMany(e => e.Orders)
                .WithOne(o => o.Employee)
                .HasForeignKey(o => o.EmployeeId));

            builder.Entity<Position>(entity =>
                entity.HasAlternateKey(e => e.Name));

            builder.Entity<Position>(entity =>
                entity.HasMany(e => e.Employees)
                .WithOne(p => p.Position)
                .HasForeignKey(p => p.PositionId));

            builder.Entity<Category>(entity =>
                entity.HasMany(e => e.Items)
                .WithOne(i => i.Category)
                .HasForeignKey(i => i.CategoryId));

            builder.Entity<Item>(entity =>
                entity.HasAlternateKey(i => i.Name));

            builder.Entity<Item>(entity =>
                entity.HasMany(i => i.OrderItems)
                .WithOne(oi => oi.Item)
                .HasForeignKey(oi => oi.ItemId));

            builder.Entity<Order>(entity =>
                entity.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId));

            builder.Entity<OrderItem>(entity =>
                entity.HasKey(oi => new { oi.OrderId, oi.ItemId }));
        }
    }
}