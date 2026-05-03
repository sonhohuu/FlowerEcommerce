namespace FlowerEcommerce.Application.Common.Configs;

public class JwtBearerConfig
{
    public JwtBearerOptions? App { get; set; }
}

public class JwtBearerOptions
{
    public string? SecretKey { get; set; }
    public string? Authority { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public ulong RefreshTokenExpiryTime { get; set; } = AppConstants.DefaultRefreshTokenExpiryTime;
    public ulong AccessTokenExpiryTime { get; set; } = AppConstants.DefaultAccessTokenExpiryTime;
}
