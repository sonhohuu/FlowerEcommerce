using FlowerEcommerce.View.Interfaces;
using FlowerEcommerce.View.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowerEcommerce.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProductApiService _productService;

        public IndexModel(IProductApiService productService)
        {
            _productService = productService;
        }

        public List<ProductViewModel> NewProducts { get; set; } = new();
        public List<ProductViewModel> FeaturedProducts { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Gọi song song, không chờ tuần tự
            var (newTask, featuredTask) = (
                _productService.GetProductsAsync(pageSize: 8),
                _productService.GetProductsAsync(pageSize: 8)
            );

            await Task.WhenAll(newTask, featuredTask);

            NewProducts = newTask.Result.Items;
            FeaturedProducts = featuredTask.Result.Items;
        }
    }
}
