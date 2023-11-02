using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users {get; set;}

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });

            modelBuilder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUser)
                .HasForeignKey(l => l.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserLike>()
                 .HasOne(s => s.LikedUser)
                 .WithMany(l => l.LikedByUser)
                 .HasForeignKey(l => l.LikedUserId)
                 .OnDelete(DeleteBehavior.NoAction);

             modelBuilder.Entity<Message>()
                 .HasOne(u => u.Recipient)
                 .WithMany(m => m.MessageReceived)
                 .OnDelete(DeleteBehavior.Restrict);

             modelBuilder.Entity<Message>()
                 .HasOne(u => u.Sender)
                 .WithMany(m => m.MessageSent)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}