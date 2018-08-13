namespace ProductsShop.Data
{
    using ProductsShop.Models;

    using Microsoft.EntityFrameworkCore;

    public class ProductsShopContext : DbContext
    {
        public ProductsShopContext() { }

        public ProductsShopContext(DbContextOptions options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<CategoryProduct> CategoriesProducts { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ModelConfigurations.CategoryConfig());
            modelBuilder.ApplyConfiguration(new ModelConfigurations.CategoryProductConfig());
            modelBuilder.ApplyConfiguration(new ModelConfigurations.ProductConfig());
            modelBuilder.ApplyConfiguration(new ModelConfigurations.UserConfig());
        }
    }
}
