using Microsoft.AspNetCore.Http;

namespace FriChat.Infrastructure.Services.CloudinaryServices
{
    public interface ICloudinary
    {
        Task<string?> UploadProfileImageAsync(IFormFile file, string folder = "profile_pictures");
        Task<string?> UploadImageFromUserAsync(IFormFile file, int userId, string folder = "Images");
        Task<string?> UploadVideoFromUserAsync(IFormFile file, int userId, string folder = "Videos");
        Task<string?> UploadAudioFromUserAsync(IFormFile file, int userId, string folder = "Audios");
        Task<string?> UploadFileFromUserAsync(IFormFile file, int userId, string folder = "Files");
        Task DeleteImageAsync(string imageUrl);
    }
}
