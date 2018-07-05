namespace AutoMapper.Data.ModelConfigurations
{
    using AutoMapper.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(e => e.Salary)
                .IsRequired();

            builder.Property(e => e.Address)
                .HasMaxLength(260)
                .IsRequired(false);

            builder.HasOne(e => e.Manager)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.ManagerId);
        }
    }
}
