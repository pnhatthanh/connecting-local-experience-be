using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Experience.Application.Interfaces;
using Experience.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Experience.Infrastructure.Services
{
    public class CloudinaryService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;

        public CloudinaryService(IOptions<CloudinarySettings> settings)
        {
            _settings = settings.Value;
            
            var account = new Account(
                _settings.CloudName,
                _settings.ApiKey,
                _settings.ApiSecret
            );
            
            _cloudinary = new Cloudinary(account);
        }

        public async Task<PhotoUploadResult> UploadPhotoAsync(IFormFile file, string? folder = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (Array.IndexOf(allowedExtensions, extension) == -1)
                throw new ArgumentException($"File type {extension} is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");

            // Validate file size (max 10MB)
            const long maxFileSize = 10 * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / 1024 / 1024}MB");

            var uploadResult = new ImageUploadResult();

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder ?? _settings.DefaultFolder,
                    Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto"),
                    UseFilename = true,
                    UniqueFilename = true
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
                throw new Exception($"Failed to upload photo: {uploadResult.Error.Message}");

            return new PhotoUploadResult
            {
                Url = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId,
                Format = uploadResult.Format,
                Size = uploadResult.Bytes,
                Width = uploadResult.Width,
                Height = uploadResult.Height
            };
        }

        public async Task<List<PhotoUploadResult>> UploadMultiplePhotosAsync(IEnumerable<IFormFile> files, string? folder = null)
        {
            var results = new List<PhotoUploadResult>();

            foreach (var file in files)
            {
                try
                {
                    var result = await UploadPhotoAsync(file, folder);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other files
                    Console.WriteLine($"Failed to upload {file.FileName}: {ex.Message}");
                }
            }

            return results;
        }

        public async Task<bool> DeletePhotoAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return false;

            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }
    }
}
