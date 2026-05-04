using FlowerEcommerce.View.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.View.Pages;

public class RegisterModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be 3–50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var client = _httpClientFactory.CreateClient("Api");
            var payload = new RegisterApiDto
            {
                Username = Input.Username,
                Email = Input.Email,
                Password = Input.Password,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
            };

            using var response = await client.PostAsJsonAsync("/api/auth/register", payload);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (response.IsSuccessStatusCode && result?.Success == true)
                return RedirectToPage("/Login", new { registered = true });

            // Hiển thị lỗi từ API (validation / duplicate username...)
            if (!result.Success)
                ErrorMessage = string.Join(" ", result.Message);
            else
                ErrorMessage = result?.Message ?? "Registration failed. Please try again.";
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
