using FlowerEcommerce.Application.Common.Configs;
using Microsoft.Extensions.Configuration;

namespace FlowerEcommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => 
        { 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // MediatR Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

        services.Configure<JwtBearerConfig>(configuration.GetSection(nameof(JwtBearerConfig)));

        return services;
    }
}
