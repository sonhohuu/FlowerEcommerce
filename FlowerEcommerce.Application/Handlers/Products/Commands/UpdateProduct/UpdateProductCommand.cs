namespace FlowerEcommerce.Application.Handlers.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<TResult>
{
    public required ulong Id { get; init; }
    public string? Name { get; init; } = null!;
    public string? Description { get; init; } = null!;
    public decimal? Price { get; init; }
    public ulong? CategoryId { get; init; }
}
