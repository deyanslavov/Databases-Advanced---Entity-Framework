namespace ProductsShop.Data.ModelConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(e => e.ProductsBought)
                .WithOne(p => p.Buyer)
                .HasForeignKey(p => p.BuyerId);

            builder.HasMany(e => e.ProductsSold)
                .WithOne(p => p.Seller)
                .HasForeignKey(p => p.SellerId);
        }
    }
}
