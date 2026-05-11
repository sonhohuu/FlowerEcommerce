using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowerEcommerce.View.Pages;

[IgnoreAntiforgeryToken]
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
        int pageNumber = 1,
        int pageSize = 10,
        bool isSelf = true,
        string? status = null,
        string? searchKeyword = null)
    {
        var client = _httpClientFactory.CreateClient("Api");

        var query = new Dictionary<string, string?>
        {
            ["Page"] = pageNumber.ToString(),
            ["PageSize"] = pageSize.ToString(),
            ["IsSelf"] = isSelf.ToString().ToLower(),
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

    public async Task<IActionResult> OnPostCancelAsync([FromBody] CancelOrderRequest body)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("userId")))
            return new JsonResult(new { success = false, message = "Chưa đăng nhập." }) { StatusCode = 401 };

        if (string.IsNullOrWhiteSpace(body?.OrderId))
            return new JsonResult(new { success = false, message = "Thiếu mã đơn hàng." }) { StatusCode = 400 };

        var client = _httpClientFactory.CreateClient("Api");

        var response = await client.DeleteAsync($"api/order/{body.OrderId}");
        var content = await response.Content.ReadAsStringAsync();

        // LOG để kiểm tra
        Console.WriteLine($"[CancelOrder] id={body.OrderId} | status={response.StatusCode} | body={content}");

        if (response.IsSuccessStatusCode)
            return new JsonResult(new { success = true });

        string errorMessage = content;
        try
        {
            var json = System.Text.Json.JsonDocument.Parse(content);
            if (json.RootElement.TryGetProperty("message", out var msg))
                errorMessage = msg.GetString() ?? content;
        }
        catch { }

        return new JsonResult(new { success = false, message = errorMessage })
        { StatusCode = (int)response.StatusCode };
    }

    public class CancelOrderRequest
    {
        public string? OrderId { get; set; }
    }
}
