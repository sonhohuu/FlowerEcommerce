using FlowerEcommerce.Application.Common.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;

namespace FlowerEcommerce.API;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtBearerConfig = configuration.GetSection(nameof(JwtBearerConfig)).Get<JwtBearerConfig>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            // options.Authority = appConfig.Authority; // If server not support OpenIdConnect, it will slow down the response
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtBearerConfig.App.Issuer,
                ValidAudience = jwtBearerConfig.App.Audience,
                // ClockSkew = TokenValidationParameters.DefaultClockSkew,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtBearerConfig.App.SecretKey))
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AppPolicy.AdminOnly, policy =>
                policy.RequireClaim(AppClaimTypes.RoleIds, (AppRoleEnum.Administrator).ToString()));

            options.AddPolicy(AppPolicy.CustomerOnly, policy =>
                policy.RequireClaim(AppClaimTypes.RoleIds, (AppRoleEnum.Customer).ToString()));

            options.AddPolicy(AppPolicy.AdminOrCustomer, policy =>
                policy.RequireClaim(AppClaimTypes.RoleIds,
                    (AppRoleEnum.Administrator).ToString(),
                    (AppRoleEnum.Customer).ToString()));
        });

        services.AddSwaggerGen(options =>
        {
            const string bearerScheme = "Bearer";
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
            options.AddSecurityDefinition(bearerScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = bearerScheme,
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(bearerScheme, document)] = []
            });
        });

        return services;
    }
}
