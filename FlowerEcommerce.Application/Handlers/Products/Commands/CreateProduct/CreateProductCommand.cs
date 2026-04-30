namespace FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<TResult>
{
    public required string Name { get; init; } = null!;
    public required string Description { get; init; } = null!;
    public decimal Price { get; init; }
    public List<IFormFile>? FileAttachMents { get; init; }
    public ulong? CategoryId { get; init; }
}
