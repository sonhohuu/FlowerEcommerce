using FlowerEcommerce.View.Interfaces;
using FlowerEcommerce.View.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FlowerEcommerce.View.Pages.Products
{
    public class DetailModel : PageModel
    {
        private readonly IProductApiService _productService;

        public ProductDetailViewModel? Product { get; set; }
        public List<ProductViewModel> RelatedProducts { get; set; } = new();
        public ReviewPaginatedResult Reviews { get; set; } = new();

        public DetailModel(IProductApiService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            Product = await _productService.GetProductDetailBySlugAsync(slug);
            if (Product is null) return Page();

            // Load trang 1 reviews song song với related (nếu có)
            Reviews = await _productService.GetProductRatingsAsync((ulong)Product.Id, page: 1, pageSize: 10);
            return Page();
        }

        // AJAX handler: POST /Products/{slug}?handler=Review
        public async Task<IActionResult> OnPostReviewAsync([FromBody] CreateReviewRequest request)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ." });

            var (success, message, statusCode) = await _productService.CreateRatingAsync(request);

            // Forward 401 về JS
            if (statusCode == 401)
                return new JsonResult(new { success = false, message = "UNAUTHORIZED" })
                {
                    StatusCode = 401
                };

            return new JsonResult(new
            {
                success,
                message = success ? "Đã gửi đánh giá!" : message
            });
        }

        // AJAX handler: GET /Products/{slug}?handler=Reviews&page=2
        public async Task<IActionResult> OnGetReviewsAsync(string slug, int page = 1)
        {
            var product = await _productService.GetProductDetailBySlugAsync(slug);
            if (product is null) return new JsonResult(new ReviewPaginatedResult());

            var result = await _productService.GetProductRatingsAsync((ulong)product.Id, page, pageSize: 10);
            return new JsonResult(result);
        }
    }
}