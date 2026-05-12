using FlowerEcommerce.Application.Common.Configs;
using FlowerEcommerce.Application.Interfaces;

namespace FlowerEcommerce.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services,
    IConfiguration configuration)
    {
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
        services.Configure<PayOSOptions>(
            configuration.GetSection(PayOSOptions.SectionName));

        var connectionString = configuration.GetConnectionString(AppConstants.DbCsKey);
        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, options) => options
            .UseSqlServer(connectionString)
            // #if DEBUG
            //             .EnableSensitiveDataLogging()
            //             .EnableDetailedErrors()
            // #endif
            .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>())
        );

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        

        services.AddScoped<IPayOSService, PayOSService>();
        services.AddScoped<IUnitOfWork,UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
    }
}
