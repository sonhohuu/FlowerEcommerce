namespace FlowerEcommerce.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services,
    IConfiguration configuration)
    {
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

        services.AddScoped<IUnitOfWork,UnitOfWork>();
        services.AddScoped<IDateTimeService, DateTimeService>();
    }
}
