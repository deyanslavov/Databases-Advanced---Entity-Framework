namespace TeamBuilder.Data
{
    using TeamBuilder.Models;
    using Microsoft.EntityFrameworkCore;

    public class TeamBuilderContext : DbContext
    {
        public TeamBuilderContext() { }

        public TeamBuilderContext(DbContextOptions options) : base(options) { }

        public DbSet<Event> Events { get; set; }

        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<EventTeam> EventsTeams { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserTeam> UsersTeams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configurations.ConnectionConfiguration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.Models.TeamConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Models.EventTeamConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Models.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Models.UserTeamConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Models.InvitationConfiguration());
        }
    }
}
