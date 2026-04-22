namespace FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<TResult>
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal Price { get; init; }
    public ulong? CategoryId { get; init; }
}
