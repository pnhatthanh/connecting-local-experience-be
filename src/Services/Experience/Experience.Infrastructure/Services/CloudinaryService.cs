using System.Diagnostics;
using BuildingBlocks.Domain.Exceptions;
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
        public async Task<string> UploadImageAsync(IFormFile imageFile, string? folder = null)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new BadRequestException("Image file is required");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                throw new BadRequestException($"File type {fileExtension} is not supported. Allowed types: {string.Join(", ", allowedExtensions)}");
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (imageFile.Length > maxFileSize)
                throw new BadRequestException($"File size exceeds maximum allowed size of {maxFileSize / 1024 / 1024}MB");

            try
            {
                using var stream = imageFile.OpenReadStream();
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    UseFilename = false, 
                    UniqueFilename = true,
                    Overwrite = false,
                    Folder = folder ?? _settings.DefaultFolder,
                    Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto") 
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                    throw new Exception($"Upload failed: {uploadResult.Error.Message}");
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error uploading image to Cloudinary: " + ex);
                throw new Exception($"Error uploading image to Cloudinary: {ex.Message}", ex);
            }
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new BadRequestException("Image URL is required");

            try
            {
                var uri = new Uri(imageUrl);
                var path = uri.AbsolutePath;
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex == -1 || uploadIndex + 1 >= segments.Length)
                    throw new BadRequestException("Invalid Cloudinary URL format");
                var publicIdParts = segments
                    .Skip(uploadIndex + 2)
                    .ToArray();

                var publicId = string.Join("/", publicIdParts);
                publicId = Path.ChangeExtension(publicId, null);
                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                };

                var result = await _cloudinary.DestroyAsync(deleteParams);
                if (result.Result != "ok")
                    throw new Exception($"Failed to delete image. Cloudinary response: {result.Result}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error deleting image from Cloudinary: " + ex);
                throw new Exception($"Error deleting image from Cloudinary: {ex.Message}", ex);
            }
        }
    }
}