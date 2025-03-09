using Application.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utils.StorageService;

namespace Utils
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUtilities(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IGoogleDriveService, GoogleDriveService>();

            return services;
        }
}
}
