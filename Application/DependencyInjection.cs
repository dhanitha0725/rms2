using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // reg all services in the project dynamically
            var assembly = typeof(DependencyInjection).Assembly;

            // register validators using FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            //register auto mapper
            services.AddAutoMapper(assembly);

            return services;
        }
    }
}
