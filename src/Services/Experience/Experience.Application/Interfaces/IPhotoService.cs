using Microsoft.AspNetCore.Http;

namespace Experience.Application.Interfaces
{
    public interface IPhotoService
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string? folder = null);
        Task DeleteImageAsync(string imageUrl);
    }
}
