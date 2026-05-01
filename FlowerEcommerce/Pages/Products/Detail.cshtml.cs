using FlowerEcommerce.View.Interfaces;
using FlowerEcommerce.View.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowerEcommerce.View.Pages.Products
{
    public class DetailModel : PageModel
    {
        private readonly IProductApiService _productService;

        public ProductDetailViewModel? Product { get; set; }
        public List<ProductViewModel> RelatedProducts { get; set; } = new();

        public DetailModel(IProductApiService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            Product = await _productService.GetProductDetailBySlugAsync(slug);
            return Page();
        }
    }
}
