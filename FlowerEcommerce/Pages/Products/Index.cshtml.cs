using FlowerEcommerce.View.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowerEcommerce.View.Pages.Products
{

    public class IndexModel : PageModel
    {
        public List<ProductViewModel> Products { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public string CurrentCategory { get; set; } = "";
        public string CurrentSort { get; set; } = "default";

        private const int PageSize = 6;

        public void OnGet(int page = 1, string sort = "default", string category = "")
        {
            CurrentPage = page;
            CurrentSort = sort;
            CurrentCategory = category;

            var query = FishingProductsData.All.AsQueryable();

            // Filter by category slug
            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.CategorySlug == category);

            // Sort
            query = sort switch
            {
                "price-asc" => query.OrderBy(p => p.IsContactPrice ? decimal.MaxValue : p.Price),
                "price-desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.Id),
                _ => query
            };

            var total = query.Count();
            TotalPages = (int)Math.Ceiling(total / (double)PageSize);

            Products = query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.Images.FirstOrDefault() ?? "",
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
                    IsContactPrice = p.IsContactPrice,
                    Category = p.Category,
                    CategorySlug = p.CategorySlug,
                })
                .ToList();
        }
    }
}
