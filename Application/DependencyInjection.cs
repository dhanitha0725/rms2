using Application.Abstractions.Interfaces;
using Application.Behaviors;
using Application.EventHandlers;
using Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // reg all services in the project dynamically
            var assembly = typeof(DependencyInjection).Assembly;

            // register mediator
            services.AddMediatR(configuration => {
                configuration.RegisterServicesFromAssembly(assembly);});

            // register validators using FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            //register auto mapper
            services.AddAutoMapper(assembly);

            // register pipeline behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

            services.AddScoped<IEmailContentService, EmailContentService>();

            return services;
        }
    }
}
