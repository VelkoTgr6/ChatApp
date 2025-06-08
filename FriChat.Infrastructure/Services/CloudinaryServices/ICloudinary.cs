using Microsoft.AspNetCore.Http;

namespace FriChat.Infrastructure.Services.CloudinaryServices
{
    public interface ICloudinary
    {
        Task<string?> UploadImageAsync(IFormFile file, string folder = "profile_pictures");
    }
}
