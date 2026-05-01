using FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

namespace FlowerEcommerce.Application.Handlers.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<TResult<ProductDto>>
{
    public required ulong Id { get; set; }

    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Product, ProductDto>()
                .Map(dest => dest.Category,
                     src => src.Category != null
                         ? new CategoryDto
                         {
                             Name = src.Category.Name,
                             Slug = src.Category.Slug
                         }
                         : null)
                .Map(dest => dest.FileAttachments,
                     src => src.FileAttachments != null
                         ? src.FileAttachments.Adapt<List<FileAttachMentDto>>()
                         : null)
                .Map(dest => dest.SizePrices,
                     src => src.ProductDetail != null
                         ? src.ProductDetail.SizePrices.Adapt<List<SizePriceDto>>()
                         : null);
        }
    }
}

public class ProductDto
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool? IsContactPrice { get; set; }
    public bool? Status { get; set; }
    public ProductDetailDto? ProductDetail { get; set; }
    public List<SizePriceDto>? SizePrices { get; set; }
    public List<FileAttachMentDto>? FileAttachments { get; set; }
    public CategoryDto? Category { get; set; }
    public bool? IsOutOfStock { get; set; }
}

public class ProductDetailDto
{
    public string? Sku { get; set; }
    public string? Slug { get; set; }
}

public class FileAttachMentDto
{
    public string? SecureUrl { get; set; }
    public string? Url { get; set; }
    public bool? IsMain { get; set; }
    public int? SortOrder { get; set; }
    public string? AltText { get; set; }
}

public class CategoryDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
}
