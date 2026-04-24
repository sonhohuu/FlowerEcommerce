namespace FlowerEcommerce.Application.Handlers.Products.Commands.DeleteProduct;

public record DeleteProductCommand : IRequest<TResult>
{
    public required ulong Id { get; init; }
}
