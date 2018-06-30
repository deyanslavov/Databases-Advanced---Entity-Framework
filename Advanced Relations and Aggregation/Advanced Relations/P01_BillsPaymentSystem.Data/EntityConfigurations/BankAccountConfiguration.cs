using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_BillsPaymentSystem.Models;

namespace P01_BillsPaymentSystem.Data.EntityConfigurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(e => e.BankAccountId);

            builder.Property(e => e.Balance)
                .IsRequired(true);

            builder.Property(e => e.BankName)
                .HasMaxLength(50)
                .IsUnicode(true);

            builder.Property(e => e.SwiftCode)
                .HasMaxLength(20)
                .IsUnicode(false);
        }
    }
}
