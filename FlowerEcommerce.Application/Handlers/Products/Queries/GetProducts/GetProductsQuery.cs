using FlowerEcommerce.Application.Handlers.Products.Queries.GetProductById;

namespace FlowerEcommerce.Application.Handlers.Products.Queries.GetProducts
{
    public class GetProductsQuery : PaginationRequest, IRequest<TResult<IPaginate<ProductListDto>>>
    {
        public string? SearchKeyword { get; set; }
        public ulong? CategoryId { get; set; }

        public class MappingConfig : IRegister
        {
            public void Register(TypeAdapterConfig config)
            {
                config.NewConfig<Product, ProductListDto>()
                    .Map(dest => dest.Slug,
                         src => src.ProductDetail != null
                             ? src.ProductDetail.Slug
                             : string.Empty)
                    .Map(dest => dest.MainImage,
                         src => src.FileAttachments
                                   .FirstOrDefault(f => f.IsMain == true)
                                   .Adapt<FileAttachMentDto>());
            }
        }
    }

    public class ProductListDto
    {
        public ulong Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public bool? IsContactPrice { get; set; }
        public bool? IsOutOfStock { get; set; }
        public bool? Status { get; set; }
        public string? Slug { get; set; }
        public FileAttachMentDto? MainImage { get; set; }
    }
}
