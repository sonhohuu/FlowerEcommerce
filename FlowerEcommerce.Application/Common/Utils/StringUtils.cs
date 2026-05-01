namespace FlowerEcommerce.Application.Common.Utils;

public static class StringUtils
{
    public static string GenerateSlug(string input)
    {
        // 1. Lowercase
        var slug = input.Trim().ToLowerInvariant();

        // 2. Bỏ dấu tiếng Việt
        slug = slug.Normalize(NormalizationForm.FormD);
        slug = new string(slug
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());
        slug = slug.Normalize(NormalizationForm.FormC);

        // 3. Thay ký tự đặc biệt còn lại bằng "-"
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // 4. Thay khoảng trắng/nhiều dấu "-" liên tiếp thành 1 dấu "-"
        slug = Regex.Replace(slug, @"[\s-]+", "-");

        // 5. Trim dấu "-" ở đầu/cuối
        return slug.Trim('-');
    }
}
