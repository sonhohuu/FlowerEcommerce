using FlowerEcommerce.View.Models;

namespace FlowerEcommerce.View.Interfaces;

public interface IProductApiService
{
    Task<(List<ProductViewModel> Items, int TotalPages, int CurrentPage)> GetProductsAsync(
        int page = 1,
        int pageSize = 12,
        ulong? categoryId = null,
        string? search = null,
        string? sort = null);

    Task<ProductDetailViewModel?> GetProductDetailBySlugAsync(string slug);
}
