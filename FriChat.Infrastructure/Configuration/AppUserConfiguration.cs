using FriChat.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriChat.Infrastructure.Configuration
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {

        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasData(new AppUser
            {
                Id = "1", // Use a GUID or string as appropriate
                UserName = "seeduser",
                FirstName = "Seed",
                LastName = "User",
                DateOfBirth = new DateTime(1990, 1, 1),
                IsDeleted = false,
                Email = "seeduser@example.com",
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                IdentityUserId = "identity-1" // Should match an IdentityUser in your Identity system
            });
        }
    }
    
}
