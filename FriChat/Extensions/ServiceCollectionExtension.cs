using FriChat.Infrastructure;
using FriChat.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FriChat.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ICloudinary, CloudinaryService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var cloudName = configuration["Cloudinary:CloudName"] ?? throw new InvalidOperationException("CloudName not configured.");
                var apiKey = configuration["Cloudinary:ApiKey"] ?? throw new InvalidOperationException("ApiKey not configured.");
                var apiSecret = configuration["Cloudinary:ApiSecret"] ?? throw new InvalidOperationException("ApiSecret not configured.");
                return new CloudinaryService(cloudName, apiKey, apiSecret);
            });

            return services;
        }

        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<FriChatDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IRepository, Repository>();

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
