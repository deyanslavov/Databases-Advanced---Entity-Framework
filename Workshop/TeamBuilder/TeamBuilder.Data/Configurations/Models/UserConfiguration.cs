namespace TeamBuilder.Data.Configurations.Models
{
    using TeamBuilder.Models;
    using Microsoft.EntityFrameworkCore;

    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.HasMany(e => e.UserTeams)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.Username)
                .IsUnique();
        }
    }
}
