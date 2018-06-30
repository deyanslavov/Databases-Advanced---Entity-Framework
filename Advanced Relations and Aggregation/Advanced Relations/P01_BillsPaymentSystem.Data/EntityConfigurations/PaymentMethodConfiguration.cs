namespace P01_BillsPaymentSystem.Data.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Models;

    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.User)
                .WithMany(u => u.PaymentMethods)
                .HasForeignKey(e => e.UserId);

            builder.HasOne(e => e.CreditCard)
                .WithMany(c => c.PaymentMethods)
                .HasForeignKey(e => e.CreditCardId);

            builder.HasOne(e => e.BankAccount)
                .WithMany(b => b.PaymentMethods)
                .HasForeignKey(e => e.BankAccountId);

            builder.HasIndex(e => new { e.UserId, e.BankAccountId, e.CreditCardId })
                .IsUnique(true);
        }
    }
}
