namespace FlowerEcommerce.Application.Handlers.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<TResult<ProductDto>>
{
    public required ulong Id { get; set; }
}
