using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Application.Abstractions.Interfaces;

namespace Persistence.Repositories
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly string _credentialPath = "C:\\Users\\dhani\\source\\repos\\rms\\client_secret_915357574098-strm8nl5flpl5nvqlt1fgetq8jqr4nth.apps.googleusercontent.com.json";
        private readonly string _folderId = "1KA4ZPyCgEjeoVGETtmODz-SbiCRTctk8";

        private DriveService GetDriveService()
        {
            var credential = GoogleCredential.FromFile(_credentialPath)
                .CreateScoped(DriveService.ScopeConstants.DriveFile);

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "rms"
            });
        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> images)
        {
            var service = GetDriveService();
            var imageUrls = new List<string>();

            foreach (var image in images)
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = $"{Guid.NewGuid()}_{image.FileName}",
                    Parents = new List<string> { _folderId }
                };

                await using var stream = image.OpenReadStream();
                var request = service.Files.Create(fileMetadata, stream, image.ContentType);
                request.Fields = "id";
                await request.UploadAsync();

                var fileId = request.ResponseBody?.Id;
                if (fileId != null)
                {
                    var fileUrl = $"https://drive.google.com/uc?id={fileId}";
                    imageUrls.Add(fileUrl);
                }
            }
            return imageUrls;
        }
    }
}