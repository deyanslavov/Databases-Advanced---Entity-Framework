namespace ProductsShop.Data.ModelConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasMany(e => e.CategoryProducts)
                .WithOne(c => c.Product)
                .HasForeignKey(c => c.ProductId);

            builder.HasOne(e => e.Buyer)
                .WithMany(b => b.ProductsBought)
                .HasForeignKey(e => e.BuyerId);

            builder.HasOne(e => e.Seller)
                .WithMany(b => b.ProductsSold)
                .HasForeignKey(e => e.SellerId);
        }
    }
}
