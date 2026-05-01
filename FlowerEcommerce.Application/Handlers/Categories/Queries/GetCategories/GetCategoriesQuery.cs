namespace FlowerEcommerce.Application.Handlers.Categories.Queries.GetCategories;

public class GetCategoriesQuery : IRequest<TResult<List<CategoryDto>>>
{

}

public class CategoryDto
{
    public ulong Id { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
}
