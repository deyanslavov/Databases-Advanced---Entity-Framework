namespace P03_FootballBetting.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_FootballBetting.Data.Models;
    using System.Collections.Generic;

    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext() { }

        public FootballBettingContext(DbContextOptions options) : base(options) { }

        public DbSet<Bet> Bets { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server =.\\SQLEXRPESS; Database = StudentSystem; Integrated Security = True");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Bet>(entity =>
            {
                entity.HasKey(e => e.BetId);

                entity.HasOne(e => e.User)
                    .WithMany(b => b.Bets)
                    .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.Game)
                    .WithMany(g => g.Bets)
                    .HasForeignKey(e => e.GameId);

                entity.Property(e => e.Prediction)
                    .IsRequired();
            });

            builder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasMany(e => e.Bets)
                    .WithOne(b => b.User)
                    .HasForeignKey(e => e.BetId);
            });

            builder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.GameId);

                entity.HasMany(e => e.Bets)
                    .WithOne(b => b.Game)
                    .HasForeignKey(e => e.BetId);

                entity.HasOne(e => e.HomeTeam)
                    .WithMany(t => t.HomeGames)
                    .HasForeignKey(e => e.HomeTeamId);

                entity.HasOne(e => e.AwayTeam)
                    .WithMany(t => t.AwayGames)
                    .HasForeignKey(e => e.AwayTeamId);
            });

            builder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.PlayerId);

                entity.HasOne(e => e.Position)
                    .WithMany(p => p.Players)
                    .HasForeignKey(e => e.PositionId);

                entity.HasOne(p => p.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TeamId);
            });

            builder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(e => new { e.GameId, e.PlayerId });

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.PlayerStatistics)
                    .HasForeignKey(e => e.PlayerId);

                entity.HasOne(e => e.Game)
                    .WithMany(g => g.PlayerStatistics)
                    .HasForeignKey(e => e.GameId);
            });

            builder.Entity<Position>(entity =>
            {
                entity.HasKey(e => e.PositionId);

                entity.HasMany(e => e.Players)
                    .WithOne(p => p.Position)
                    .HasForeignKey(e => e.PlayerId);
            });

            builder.Entity<Town>(entity =>
            {
                entity.HasKey(e => e.TownId);

                entity.HasOne(e => e.Country)
                    .WithMany(c => c.Towns)
                    .HasForeignKey(e => e.CountryId);

                entity.HasMany(e => e.Teams)
                    .WithOne(t => t.Town)
                    .HasForeignKey(e => e.TeamId);
            });

            builder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.CountryId);

                entity.HasMany(e => e.Towns)
                    .WithOne(t => t.Country)
                    .HasForeignKey(e => e.TownId);
            });

            builder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.ColorId);

                //entity.HasMany(e => e.SecondaryKitTeams)
                //    .WithOne(t => t.SecondaryKitColor)
                //    .HasForeignKey(e => e.SecondaryKitColorId);

                //entity.HasMany(e => e.PrimaryKitTeams)
                //    .WithOne(t => t.PrimaryKitColor)
                //    .HasForeignKey(e => e.PrimaryKitColor);
            });

            builder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.TeamId);

                entity.HasOne(e => e.PrimaryKitColor)
                    .WithMany(c => c.PrimaryKitTeams)
                    .HasForeignKey(e => e.PrimaryKitColorId);

                entity.HasOne(e => e.SecondaryKitColor)
                    .WithMany(c => c.SecondaryKitTeams)
                    .HasForeignKey(e => e.SecondaryKitColorId);

                entity.HasMany(e => e.HomeGames)
                    .WithOne(g => g.HomeTeam)
                    .HasForeignKey(e => e.HomeTeamId);

                entity.HasMany(e => e.AwayGames)
                    .WithOne(g => g.AwayTeam)
                    .HasForeignKey(e => e.AwayTeamId);

                entity.HasOne(e => e.Town)
                    .WithMany(t => t.Teams)
                    .HasForeignKey(e => e.TownId);

                entity.HasMany(e => e.Players)
                    .WithOne(p => p.Team)
                    .HasForeignKey(e => e.PlayerId);
            });
        }
    }
}
