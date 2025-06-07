using Microsoft.AspNetCore.Http;

namespace FriChat.Infrastructure.Data.Common
{
    public interface ICloudinary
    {
        Task<string?> UploadImageAsync(IFormFile file, string folder = "profile_pictures");
    }
}
