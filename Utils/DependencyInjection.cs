using Application.Abstractions.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utilities.BackgroundJobs;
using Utilities.EmailService;
using Utilities.PaymentGateway;
using Utilities.StorageService;

namespace Utilities
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUtilities(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IGoogleDriveService, GoogleDriveService>();

            // Add SMTP email service configuration
            var smtpServer = configuration["Smtp:Server"];
            var smtpPort = int.Parse(configuration["Smtp:Port"]);
            var senderEmail = configuration["Smtp:SenderEmail"];
            var senderPassword = configuration["Smtp:SenderPassword"];
            var secureSocketOptions = Enum.Parse<SecureSocketOptions>(configuration["Smtp:SecureSocketOptions"]);

            services.AddSingleton<IEmailService>(provider =>
                new SmtpEmailService(smtpServer, smtpPort, senderEmail, senderPassword, secureSocketOptions));

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            // register blob services
            var blobConnection = configuration.GetConnectionString("AzureBlobStorageConnectionString");
            services.AddSingleton<IBlobService>(new BlobService(blobConnection));

            // register payment services
            services.Configure<PayhereSettings>(configuration.GetSection("PayhereSettings"));
            services.AddScoped<IPayhereService, PayhereService>();

            return services;
        }
    }
}
