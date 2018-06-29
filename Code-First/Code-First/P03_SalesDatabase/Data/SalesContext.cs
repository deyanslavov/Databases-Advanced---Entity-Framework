namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using P03_SalesDatabase.Data.Models;

    public class SalesContext : DbContext
    {
        public SalesContext()
        {

        }

        public SalesContext(DbContextOptions options)
            :base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<Store> Stores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=Sales;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Customer
            
            builder.Entity<Customer>()
                .ToTable("Customers")
                .HasKey(c => c.CustomerId);

            builder.Entity<Customer>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsUnicode();

            builder.Entity<Customer>()
                .Property(c => c.Email)
                .HasMaxLength(80);

            builder.Entity<Customer>()
                .HasMany(c => c.Sales)
                .WithOne(s => s.Customer)
                .HasForeignKey(s => s.CustomerId);

            // Product

            builder.Entity<Product>()
                .ToTable("Products")
                .HasKey(p => p.ProductId);

            builder.Entity<Product>()
                .Property(p => p.Description)
                .HasMaxLength(250)
                .HasDefaultValue("No description");

            builder.Entity<Product>()
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsUnicode();

            builder.Entity<Product>()
                .HasMany(p => p.Sales)
                .WithOne(s => s.Product)
                .HasForeignKey(s => s.ProductId);

            // Sale

            builder.Entity<Sale>()
                .ToTable("Sales")
                .HasKey(s => s.SaleId);

            builder.Entity<Sale>()
                .Property(s => s.Date)
                .HasDefaultValueSql("GETDATE()");

            // Store

            builder.Entity<Store>()
                .ToTable("Stores")
                .HasKey(s => s.StoreId);

            builder.Entity<Store>()
                .Property(s => s.Name)
                .HasMaxLength(80)
                .IsUnicode();

            builder.Entity<Store>()
                .HasMany(s => s.Sales)
                .WithOne(sale => sale.Store)
                .HasForeignKey(sale => sale.StoreId);
        }
    }
}
