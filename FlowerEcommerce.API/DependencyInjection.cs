using FlowerEcommerce.Application.Common.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

        services.AddAuthorization();

        return services;
    }
}
