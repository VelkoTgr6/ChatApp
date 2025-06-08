using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace FriChat.Infrastructure.Services.CloudinaryServices
{
    public class CloudinaryService : ICloudinary
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(string cloudName, string apiKey, string apiSecret)
        {
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string?> UploadImageAsync(IFormFile file, string folder = "profile_pictures")
        {
            if (file == null || file.Length == 0)
                return null;

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation().Width(200).Height(200).Crop("fill").Quality("auto:low")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.StatusCode == System.Net.HttpStatusCode.OK
                ? uploadResult.SecureUrl.ToString()
                : null;
        }
    }
}
