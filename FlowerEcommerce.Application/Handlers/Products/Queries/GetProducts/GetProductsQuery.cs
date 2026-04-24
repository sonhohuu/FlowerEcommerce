
namespace FlowerEcommerce.Application.Handlers.Products.Queries.GetProducts
{
    public class GetProductsQuery : PaginationRequest, IRequest<TResult<IPaginate<ProductDto>>>
    {
        public string? SearchKeyword { get; init; }
    }

    public class ProductDto
    {
        public ulong Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string? CategoryName { get; set; }
    }
}
