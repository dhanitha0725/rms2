﻿using Application.Abstractions.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Utilities.StorageService
{
    public class GoogleDriveService(ILogger logger) : IGoogleDriveService
    {
        private readonly string _credentialPath = "C:\\Users\\dhani\\source\\repos\\rms\\polished-core-451817-b1-5f4e27a4e879.json";
        private readonly string _folderId = "1KA4ZPyCgEjeoVGETtmODz-SbiCRTctk8";

        private DriveService GetDriveService()
        {
            try
            {
                var credential = GoogleCredential.FromFile(_credentialPath)
                    .CreateScoped(DriveService.ScopeConstants.DriveFile);

                return new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "rms"
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error creating Google Drive service.");
                throw;
            }
        }

        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files)
        {
            var service = GetDriveService();
            var fileUrls = new List<string>();

            foreach (var file in files)
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = $"{Guid.NewGuid()}_{file.FileName}",
                    Parents = new List<string> { _folderId }
                };

                await using var stream = file.OpenReadStream();
                var request = service.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "id";
                await request.UploadAsync();

                var fileId = request.ResponseBody?.Id;
                if (fileId != null)
                {

                    //make the file public
                    var permission = new Google.Apis.Drive.v3.Data.Permission
                    {
                        Type = "anyone",
                        Role = "reader"
                    };

                    await service.Permissions.Create(permission, fileId).ExecuteAsync();
                    
                    var fileUrl = $"https://drive.google.com/uc?id={fileId}";
                    fileUrls.Add(fileUrl);
                }
            }

            return fileUrls;
        }
    }
}