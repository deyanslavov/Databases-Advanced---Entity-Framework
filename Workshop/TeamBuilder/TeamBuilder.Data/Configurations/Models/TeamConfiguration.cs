namespace TeamBuilder.Data.Configurations.Models
{
    using TeamBuilder.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasMany(e => e.UserTeams)
                .WithOne(t => t.Team)
                .HasForeignKey(t => t.TeamId);

            builder.HasMany(e => e.EventTeams)
                .WithOne(t => t.Team)
                .HasForeignKey(t => t.TeamId);

            builder.HasOne(e => e.Creator)
                .WithMany(c => c.CreatedTeams)
                .HasForeignKey(e => e.CreatorId);
        }
    }
}
