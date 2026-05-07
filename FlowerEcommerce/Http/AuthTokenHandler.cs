using FlowerEcommerce.View.Models;
using System.Net;
using System.Net.Http.Headers;

namespace FlowerEcommerce.View.Http;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthTokenHandler> _logger;
    private readonly IHostEnvironment _env;

    public AuthTokenHandler(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration config,
        ILogger<AuthTokenHandler> logger,
        IHostEnvironment env)
    {
        _httpContextAccessor = httpContextAccessor;
        _config = config;
        _logger = logger;
        _env = env;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var accessToken = httpContext?.Request.Cookies["access_token"];

        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogInformation("401 received, attempting token refresh...");
            var newToken = await TryRefreshTokenAsync(httpContext, cancellationToken);

            if (newToken is not null)
            {
                var retryRequest = await CloneRequestAsync(request);
                retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                response = await base.SendAsync(retryRequest, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Token refresh failed.");
            }
        }

        return response;
    }

    private async Task<string?> TryRefreshTokenAsync(
        HttpContext? httpContext, CancellationToken cancellationToken)
    {
        if (httpContext is null) return null;

        var refreshToken = httpContext.Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("No refresh_token cookie found.");
            return null;
        }

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_config["ApiBaseUrl"]!);

            using var refreshResponse = await client.PostAsJsonAsync(
                "api/auth/refresh-token",
                new { RefreshToken = refreshToken },
                CancellationToken.None);

            _logger.LogInformation("Refresh API response: {Status}", refreshResponse.StatusCode);

            if (!refreshResponse.IsSuccessStatusCode)
            {
                DeleteAuthCookies(httpContext);
                return null;
            }

            var result = await refreshResponse.Content
                .ReadFromJsonAsync<ApiResponse<TokenModel>>(cancellationToken: CancellationToken.None);

            if (result?.Data is null) return null;
            var token = result.Data;

            // Dev  → Secure=false  → cookie hoạt động trên http://localhost
            // Prod → Secure=true   → bắt buộc https
            var secure = !_env.IsDevelopment();

            var accessCookieOpts = new CookieOptions
            {
                HttpOnly = true,
                Secure = secure,
                SameSite = SameSiteMode.Lax,
                Expires = token.AccessTokenExpires
            };

            var refreshCookieOpts = new CookieOptions
            {
                HttpOnly = true,
                Secure = secure,
                SameSite = SameSiteMode.Lax,
                Expires = token.RefreshTokenExpires
            };

            httpContext.Response.Cookies.Append("access_token", token.AccessToken ?? "", accessCookieOpts);

            if (!string.IsNullOrEmpty(token.RefreshToken))
                httpContext.Response.Cookies.Append("refresh_token", token.RefreshToken, refreshCookieOpts);

            _logger.LogInformation("Tokens refreshed and saved to cookies.");
            return token.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during token refresh");
            return null;
        }
    }

    private static void DeleteAuthCookies(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("access_token");
        httpContext.Response.Cookies.Delete("refresh_token");
        httpContext.Session.Remove("username");
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);

        if (original.Content is not null)
        {
            var bytes = await original.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(bytes);
            foreach (var header in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var header in original.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }
}