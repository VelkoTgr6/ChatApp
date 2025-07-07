using FriChat.Infrastructure.Services.CloudinaryServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FriChat.Infrastructure.Services.MediaCleanup
{
    public class MediaCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public MediaCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<FriChatDbContext>();
                    var cloudinary = scope.ServiceProvider.GetRequiredService<ICloudinary>();
                    var expiredMedia = dbContext.UserMedias
                        .Where(m => m.UploadedAt < DateTime.UtcNow.AddDays(-2))
                        .ToList();

                    foreach (var media in expiredMedia)
                    {
                        // Delete from Cloudinary
                        await cloudinary.DeleteImageAsync(media.Url);

                        // Remove from DB
                        dbContext.UserMedias.Remove(media);
                    }

                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Run every hour
            }
        }
    }
}
