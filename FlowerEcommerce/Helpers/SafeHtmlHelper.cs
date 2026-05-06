using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ganss.Xss;

namespace FlowerEcommerce.View.Helpers;

public static class SafeHtmlHelper
{
    private static readonly HtmlSanitizer _sanitizer = new HtmlSanitizer();

    static SafeHtmlHelper()
    {
        // Chỉ cho phép các tag an toàn — giống DOMPurify config của bạn
        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedTags.Add("b");
        _sanitizer.AllowedTags.Add("strong");
        _sanitizer.AllowedTags.Add("i");
        _sanitizer.AllowedTags.Add("em");
        _sanitizer.AllowedTags.Add("br");
        _sanitizer.AllowedTags.Add("ul");
        _sanitizer.AllowedTags.Add("ol");
        _sanitizer.AllowedTags.Add("li");
        _sanitizer.AllowedTags.Add("p");

        // Không cho phép attribute nào (tránh onclick, style inject...)
        _sanitizer.AllowedAttributes.Clear();
    }

    public static IHtmlContent SafeHtml(this IHtmlHelper helper, string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return HtmlString.Empty;

        var clean = _sanitizer.Sanitize(html);
        return new HtmlString(clean);
    }
}
