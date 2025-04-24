using Application.Abstractions.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Utilities.StorageService
{
    public class BlobService(string connectionString) : IBlobService
    {
        private async Task<BlobContainerClient> GetBlobContainer(string containerName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return containerClient;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var container = await GetBlobContainer(containerName);
            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            await using var stream = file.OpenReadStream();
            var blobClient = container.GetBlobClient(blobName);
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });
            return blobName;
        }

        public async Task DeleteFileAsync(string fileName, string containerName)
        {
            var container = await GetBlobContainer(containerName);
            var blobClient = container.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<Stream> GetFileAsync(string fileName, string containerName)
        {
            var container = await GetBlobContainer(containerName);
            var blobClient = container.GetBlobClient(fileName);
            var response = await blobClient.DownloadContentAsync();
            return response.Value.Content.ToStream();
        }

        public async Task<string> GetFileUrl(string fileName, string containerName)
        {
           var container = await GetBlobContainer(containerName);
           var blobClient = container.GetBlobClient(fileName);
           return blobClient.Uri.ToString();
        }
    }
}
