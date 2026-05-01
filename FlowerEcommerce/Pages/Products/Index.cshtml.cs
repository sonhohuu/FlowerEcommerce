using FlowerEcommerce.View.Interfaces;
using FlowerEcommerce.View.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowerEcommerce.View.Pages.Products;


public class IndexModel : PageModel
{
    private readonly IProductApiService _productService;
    private readonly ICategoryApiService _categoryService;

    public IndexModel(IProductApiService productService, ICategoryApiService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    // ── Bound data for the view ──────────────────────────────────────────────
    public List<ProductViewModel> Products { get; private set; } = new();
    public List<CategoryViewModel> Categories { get; private set; } = new();
    public int TotalPages { get; private set; }
    public int CurrentPage { get; private set; }

    public async Task OnGetAsync(
        int page = 1,
        string? category = null,   // slug from URL query string
        string? search = null,
        string? sort = null)
    {
        // 1. Fetch categories (used for sidebar + slug→id lookup)
        Categories = await _categoryService.GetCategoriesAsync();

        // 2. Resolve slug → categoryId
        ulong? categoryId = null;
        if (!string.IsNullOrEmpty(category))
        {
            var matched = Categories.FirstOrDefault(
                c => string.Equals(c.Slug, category, StringComparison.OrdinalIgnoreCase));
            categoryId = matched?.Id;
        }

        // 3. Fetch products with the resolved ID
        var (items, totalPages, currentPage) = await _productService.GetProductsAsync(
            page: page,
            categoryId: categoryId,
            search: search,
            sort: sort);

        Products = items;
        TotalPages = totalPages;
        CurrentPage = currentPage;
    }
}
