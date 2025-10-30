using Microsoft.AspNetCore.Http;

namespace User.Application.Interfaces
{
    public interface IPhotoService
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string? folder = null);
        Task DeleteImageAsync(string imageUrl);
    }
}
