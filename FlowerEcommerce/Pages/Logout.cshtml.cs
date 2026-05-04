using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowerEcommerce.View.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // Xóa session
        HttpContext.Session.Clear();

        // Xóa cookies token
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return RedirectToPage("/Index");
    }
}
