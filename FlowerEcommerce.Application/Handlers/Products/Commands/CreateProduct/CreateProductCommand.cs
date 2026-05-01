namespace FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<TResult>
{
    public required string Name { get; init; } = null!;
    public required string Description { get; init; } = null!;
    public decimal Price { get; init; }
    public decimal? OriginalPrice { get; init; }
    public bool IsContactPrice { get; init; } = false;
    public string Sku { get; init; } = string.Empty;
    public IList<SizePriceDto>? SizePrices { get; set; }
    public List<IFormFile>? FileAttachMents { get; init; }
    public ulong? CategoryId { get; init; }
}

public record SizePriceDto
{
    public string Label { get; init; } = null!;
    public decimal Price { get; init; }
}



