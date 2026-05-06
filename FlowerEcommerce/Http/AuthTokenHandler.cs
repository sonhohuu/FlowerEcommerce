using FlowerEcommerce.View.Models;
using System.Net;
using System.Net.Http.Headers;

namespace FlowerEcommerce.View.Http;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _config;

    public AuthTokenHandler(IHttpContextAccessor httpContextAccessor, IConfiguration config)
    {
        _httpContextAccessor = httpContextAccessor;
        _config = config;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var accessToken = httpContext?.Request.Cookies["access_token"];

        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        // Access token hết hạn → thử refresh
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var newToken = await TryRefreshTokenAsync(httpContext, cancellationToken);
            if (newToken is not null)
            {
                // Clone request vì HttpRequestMessage không thể gửi lại
                var retryRequest = await CloneRequestAsync(request);
                retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                response = await base.SendAsync(retryRequest, cancellationToken);
            }
        }

        return response;
    }

    private async Task<string?> TryRefreshTokenAsync(
        HttpContext? httpContext, CancellationToken cancellationToken)
    {
        if (httpContext is null) return null;

        var refreshToken = httpContext.Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken)) return null;

        // Gọi thẳng không qua handler (tránh vòng lặp vô tận)
        using var client = new HttpClient();
        client.BaseAddress = new Uri(_config["ApiBaseUrl"]!);

        using var refreshResponse = await client.PostAsJsonAsync(
            "/api/auth/refresh-token",
            new { RefreshToken = refreshToken },
            cancellationToken);

        if (!refreshResponse.IsSuccessStatusCode)
        {
            // Refresh token cũng hết hạn → xóa cookie, bắt login lại
            httpContext.Response.Cookies.Delete("access_token");
            httpContext.Response.Cookies.Delete("refresh_token");
            return null;
        }

        var result = await refreshResponse.Content
            .ReadFromJsonAsync<ApiResponse<LoginData>>(cancellationToken: cancellationToken);

        if (result?.Data?.TokenModel is null) return null;

        var token = result.Data.TokenModel;

        // Cập nhật cookie mới
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = token.AccessTokenExpires
        };

        httpContext.Response.Cookies.Append("access_token", token.AccessToken ?? "", cookieOptions);

        if (!string.IsNullOrEmpty(token.RefreshToken))
        {
            httpContext.Response.Cookies.Append("refresh_token", token.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = token.RefreshTokenExpires
            });
        }

        return token.AccessToken;
    }

    // HttpRequestMessage không reusable → phải clone trước khi retry
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
