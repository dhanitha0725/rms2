using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces
{
    public interface IGoogleDriveService
    {
        Task<List<string>> UploadFilesAsync(List<IFormFile> images);
    }
}
