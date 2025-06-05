using FriChat.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FriChat.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            // Add your custom services here
            // Example: services.AddScoped<IMyService, MyService>();
            // If you have a DbContext, add it here
            // Example: services.AddDbContext<FriChatDbContext>(options => options.UseNpgsql("YourConnectionString"));
            // Add Identity services if needed
            // Example: services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<FriChatDbContext>();
            return services;
        }

        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<FriChatDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddRazorPages();

            return services;
        }

        public static IServiceCollection AddAppIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<FriChatDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
