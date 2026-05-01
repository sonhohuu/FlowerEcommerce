using FlowerEcommerce.View.Models;

namespace FlowerEcommerce.View.Interfaces;

public interface ICategoryApiService
{
    Task<List<CategoryViewModel>> GetCategoriesAsync();
}
