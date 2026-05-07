using FlowerEcommerce.View.Interfaces;
using FlowerEcommerce.View.Models;

namespace FlowerEcommerce.View.Services;

public class CategoryApiService : ICategoryApiService
{
    private readonly HttpClient _httpClient;

    public CategoryApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<List<CategoryApiDto>>>("api/category");

        if (response?.Success != true || response.Data is null)
            return new List<CategoryViewModel>();

        return response.Data
            .Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
            })
            .ToList();
    }
}
