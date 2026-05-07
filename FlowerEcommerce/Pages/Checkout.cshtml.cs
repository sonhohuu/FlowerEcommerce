using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowerEcommerce.View.Pages;

[IgnoreAntiforgeryToken]
public class CheckoutModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CheckoutModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    // Proxy: POST /Checkout?handler=PlaceOrder
    // Body: { customerName, phoneNumber, address, note, paymentMethod, items: [...] }
    public async Task<IActionResult> OnPostPlaceOrderAsync()
    {
        var client = _httpClientFactory.CreateClient("Api");

        Console.WriteLine($"[DEBUG] BaseAddress = {client.BaseAddress}");
        Console.WriteLine($"[DEBUG] Target URL  = {new Uri(client.BaseAddress!, "api/order")}");

        // Đọc raw body từ request rồi forward thẳng xuống API
        using var reader = new StreamReader(Request.Body);
        var bodyJson = await reader.ReadToEndAsync();

        var content = new StringContent(bodyJson, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("api/order", content);
        var result = await response.Content.ReadAsStringAsync();

        // Trả về status code gốc từ API kèm JSON body
        return new ContentResult
        {
            Content = result,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode
        };
    }
}
