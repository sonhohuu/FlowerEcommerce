using FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

namespace FlowerEcommerce.Application.Handlers.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<TResult>
{
    [SwaggerIgnore]
    public required ulong Id { get; set; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public decimal? OriginalPrice { get; init; }
    public bool? IsContactPrice { get; init; }
    public string? Sku { get; init; } = string.Empty;
    public IList<SizePriceDto>? SizePrices { get; set; }
    public List<IFormFile>? FileAttachMents { get; init; }
    public ulong? CategoryId { get; init; }

    private class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UpdateProductCommand, Product>()
                .IgnoreNullValues(true);
        }
    }
}


