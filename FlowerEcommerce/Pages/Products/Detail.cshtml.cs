using FlowerEcommerce.View.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowerEcommerce.View.Pages.Products
{
    public class DetailModel : PageModel
    {
        public ProductDetailViewModel? Product { get; set; }
        public List<ProductViewModel> RelatedProducts { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            Product = FishingProductsData.All.FirstOrDefault(p => p.Id == id);

            if (Product == null)
                return Page(); // View will show "không tìm thấy"

            // Related: same category, excluding current product, max 6
            RelatedProducts = FishingProductsData.All
                .Where(p => p.CategorySlug == Product.CategorySlug && p.Id != id)
                .Take(6)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.Images.FirstOrDefault() ?? "",
                    Price = p.Price,
                    IsContactPrice = p.IsContactPrice,
                    Category = p.Category,
                    CategorySlug = p.CategorySlug,
                })
                .ToList();

            return Page();
        }
    }
}
