using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Project.Application.Behaviors;

namespace Project.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Register MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(Project.Application.Behaviors.ValidationBehavior<,>));
        });

        // Register FluentValidation
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        // Register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
