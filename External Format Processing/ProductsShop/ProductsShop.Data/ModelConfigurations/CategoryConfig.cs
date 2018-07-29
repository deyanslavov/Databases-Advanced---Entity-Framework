namespace ProductsShop.Data.ModelConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasMany(e => e.CategoryProducts)
                .WithOne(c => c.Category)
                .HasForeignKey(c => c.CategoryId);
        }
    }
}
