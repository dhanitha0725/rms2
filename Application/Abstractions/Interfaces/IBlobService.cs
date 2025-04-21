using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file, string containerName);
        Task DeleteFileAsync(string fileName, string containerName);
        Task<Stream> GetFileAsync(string fileName, string containerName);
        Task<string> GetFileUrl(string fileName, string containerName);
    }
}
