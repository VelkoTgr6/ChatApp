using FriChat.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FriChat.Infrastructure
{
    public class FriChatDbContext : IdentityDbContext
    {
        public FriChatDbContext(DbContextOptions<FriChatDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Friends)
                .WithMany(u => u.FriendOf)
                .UsingEntity<Dictionary<string, object>>(
                "UserFriend",
                j => j
                    .HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey("FriendId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                    {
                        j.HasKey("UserId", "FriendId");
                        j.ToTable("UserFriends");
                    });

            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.FriendRequests)
                .WithMany(u => u.ReceivedFriendRequests)
                .UsingEntity<Dictionary<string, object>>(
                "UserFriendRequest",
                j => j
                    .HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey("RequestedUserId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey("RequestingUserId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("RequestingUserId", "RequestedUserId");
                    j.ToTable("UserFriendRequests");
                });
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
    }

}
