using FlowerEcommerce.View.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.View.Pages;

public class LoginModel : PageModel 
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public IActionResult OnGet(bool registered = false)
    {
        if (registered)
            SuccessMessage = "Account created successfully. Please log in.";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var client = _httpClientFactory.CreateClient("Api");
            var endpoint = $"api/auth/login";

            var payload = new LoginApiDto { Username = Input.UserName, Password = Input.Password };
            using var response = await client.PostAsJsonAsync(endpoint, payload);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginData>>();

            if (response.IsSuccessStatusCode && result?.Success == true && result.Data?.TokenModel != null)
            {
                var token = result.Data.TokenModel;
                // accessToken — HttpOnly, expire theo thời gian server trả về
                Response.Cookies.Append("access_token", token.AccessToken ?? "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = Input.RememberMe
                        ? token.AccessTokenExpires
                        : null   // session cookie nếu không nhớ
                });

                // refreshToken — expire theo refreshTokenExpires
                if (!string.IsNullOrEmpty(token.RefreshToken))
                {
                    Response.Cookies.Append("refresh_token", token.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = token.RefreshTokenExpires
                    });
                }

                // Lưu thêm thông tin user vào Session (không nhạy cảm)
                HttpContext.Session.SetString("username", result.Data.UserName ?? "");
                HttpContext.Session.SetString("allowedRole", result.Data.AllowedRole ?? "");
                HttpContext.Session.SetString("userId", result.Data.UserId.ToString());

                return RedirectToPage("/Index");
            }

            // API trả về success: false hoặc HTTP lỗi
            ErrorMessage = result?.Message ?? "Invalid username or password.";
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Cannot connect to the server. Please try again later.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unexpected error: {ex.Message}";
        }

        return Page();
    }
}
