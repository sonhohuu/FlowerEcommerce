using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowerEcommerce.View.Pages;

public class OrderHistoryModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OrderHistoryModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("userId")))
            return RedirectToPage("/Login", new { returnUrl = "/OrderHistory" });

        return Page();
    }

    // Proxy: GET /OrderHistory?handler=Orders&page=1&size=10&isSelf=true&status=...&searchKeyword=...
    public async Task<IActionResult> OnGetOrdersAsync(
        int page = 1,
        int size = 10,
        bool isSelf = true,
        string? status = null,
        string? searchKeyword = null)
    {
        var client = _httpClientFactory.CreateClient("Api");

        var query = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(),
            ["size"] = size.ToString(),
            ["isSelf"] = isSelf.ToString().ToLower(),
        };

        if (!string.IsNullOrEmpty(status))
            query["status"] = status;

        if (!string.IsNullOrEmpty(searchKeyword))
            query["searchKeyword"] = searchKeyword;

        var qs = string.Join("&", query
            .Where(x => x.Value != null)
            .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));

        var response = await client.GetAsync($"api/order?{qs}");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json");
    }
}
