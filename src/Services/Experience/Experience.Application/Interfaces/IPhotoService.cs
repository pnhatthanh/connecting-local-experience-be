using Microsoft.AspNetCore.Http;

namespace Experience.Application.Interfaces
{
    public interface IPhotoService
    {
        Task<PhotoUploadResult> UploadPhotoAsync(IFormFile file, string? folder = null);
        Task<bool> DeletePhotoAsync(string publicId);
        Task<List<PhotoUploadResult>> UploadMultiplePhotosAsync(IEnumerable<IFormFile> files, string? folder = null);
    }

    public class PhotoUploadResult
    {
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public long Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
