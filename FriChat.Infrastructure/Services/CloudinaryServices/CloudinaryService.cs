using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace FriChat.Infrastructure.Services.CloudinaryServices
{
    public class CloudinaryService : ICloudinary
    {
        private readonly Cloudinary cloudinary;

        public CloudinaryService(string cloudName, string apiKey, string apiSecret)
        {
            var account = new Account(cloudName, apiKey, apiSecret);
            cloudinary = new Cloudinary(account);
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            // Extract public ID from the URL
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2)
                return;

            // The public ID is everything after the folder, without the extension
            // e.g. .../profile_pictures/abc123.jpg => profile_pictures/abc123
            var folder = segments[segments.Length - 2];
            var fileWithExt = segments[segments.Length - 1];
            var publicId = $"{folder}/{System.IO.Path.GetFileNameWithoutExtension(fileWithExt)}";

            var deletionParams = new DeletionParams(publicId);
            await cloudinary.DestroyAsync(deletionParams);
        }

        public async Task<string?> UploadProfileImageAsync(IFormFile file, string folder = "profile_pictures")
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

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return uploadResult.StatusCode == System.Net.HttpStatusCode.OK
                ? uploadResult.SecureUrl.ToString()
                : null;
        }

        public async Task<string?> UploadImageFromUserAsync(IFormFile file, int userId, string folder = "Images")
        {
            if (file == null || file.Length == 0)
                return null;

            var userFolder = $"user_{userId}/{folder}";
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = userFolder,
                Transformation = new Transformation().Width(200).Height(200).Crop("fill").Quality("auto:low")
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return uploadResult.StatusCode == System.Net.HttpStatusCode.OK
                ? uploadResult.SecureUrl.ToString()
                : null;
        }

        public async Task<string?> UploadVideoFromUserAsync(IFormFile file, int userId, string folder = "Videos")
        {
            if (file == null || file.Length == 0)
                return null;

            // Limit file size (e.g., 50MB)
            const long maxSize = 50 * 1024 * 1024;
            if (file.Length > maxSize)
                throw new InvalidOperationException("Video file is too large. Maximum allowed size is 50MB.");

            var userFolder = $"user_{userId}/{folder}";
            using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = userFolder,
                // Limit resolution and bitrate for optimization
                Transformation = new Transformation().Width(720).Height(480).Crop("limit").BitRate("1m")
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return uploadResult.StatusCode == System.Net.HttpStatusCode.OK
                ? uploadResult.SecureUrl.ToString()
                : null;
        }

        public async Task<string?> UploadAudioFromUserAsync(IFormFile file, int userId, string folder = "Audios")
        {
            if (file == null || file.Length == 0)
                return null;

            // Limit file size (e.g., 10MB)
            const long maxSize = 10 * 1024 * 1024;
            if (file.Length > maxSize)
                throw new InvalidOperationException("Audio file is too large. Maximum allowed size is 10MB.");

            var userFolder = $"user_{userId}/{folder}";
            using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = userFolder,
                // You can add eager transformations for audio conversion if needed
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return uploadResult.StatusCode == System.Net.HttpStatusCode.OK
                ? uploadResult.SecureUrl.ToString()
                : null;
        }

        public async Task<string?> UploadFileFromUserAsync(IFormFile file, int userId, string folder = "Files")
        {
            if (file == null || file.Length == 0)
                return null;

            // Limit file size (e.g., 20MB)
            const long maxSize = 20 * 1024 * 1024;
            if (file.Length > maxSize)
                throw new InvalidOperationException("File is too large. Maximum allowed size is 20MB.");

            var userFolder = $"user_{userId}/{folder}";
            using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = userFolder
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return uploadResult.StatusCode == System.Net.HttpStatusCode.OK
                ? uploadResult.SecureUrl.ToString()
                : null;
        }
    }
}
