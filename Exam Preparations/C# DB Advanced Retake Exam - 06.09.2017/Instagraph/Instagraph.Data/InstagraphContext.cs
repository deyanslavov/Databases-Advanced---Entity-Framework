namespace Instagraph.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;

    public class InstagraphContext : DbContext
    {
        public InstagraphContext() { }

        public InstagraphContext(DbContextOptions options)
            :base(options) { }

        public DbSet<Picture> Pictures { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<UserFollower> UsersFollowers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Picture>(e =>
                e.HasMany(p => p.Users)
                .WithOne(u => u.ProfilePicture)
                .HasForeignKey(u => u.ProfilePictureId));

            builder.Entity<Picture>(e =>
                e.HasMany(p => p.Posts)
                .WithOne(u => u.Picture)
                .HasForeignKey(u => u.PictureId));

            builder.Entity<User>(e =>
                e.HasAlternateKey(u => u.Username));

            builder.Entity<User>(e =>
                e.HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict));

            builder.Entity<User>(e =>
                e.HasMany(u => u.Comments)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId));

            builder.Entity<User>(e =>
                e.HasMany(u => u.UsersFollowing)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict));

            builder.Entity<User>(e =>
                e.HasMany(u => u.Followers)
                .WithOne(p => p.Follower)
                .HasForeignKey(p => p.FollowerId)
                .OnDelete(DeleteBehavior.Restrict));

            builder.Entity<Post>(e =>
                e.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict));

            builder.Entity<UserFollower>(e =>
                e.HasKey(uf => new { uf.FollowerId, uf.UserId }));
        }
    }
}
