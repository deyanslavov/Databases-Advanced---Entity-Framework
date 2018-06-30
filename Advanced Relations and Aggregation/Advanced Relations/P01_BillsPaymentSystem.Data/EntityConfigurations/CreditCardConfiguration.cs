namespace P01_BillsPaymentSystem.Data.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Models;

    public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            builder.HasKey(e => e.CreditCardId);

            builder.Ignore(e => e.LimitLeft);

            builder.Property(e => e.Limit)
                .IsRequired();

            builder.Property(e => e.MoneyOwed)
                .IsRequired();

            builder.Property(e => e.ExpirationDate)
                .IsRequired();
        }
    }
}
