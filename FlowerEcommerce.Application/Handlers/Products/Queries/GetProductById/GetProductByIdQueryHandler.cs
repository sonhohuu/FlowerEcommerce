using FlowerEcommerce.Application.Handlers.Products.Queries.GetProducts;

namespace FlowerEcommerce.Application.Handlers.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, TResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;
    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetProductByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<TResult<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Repository<Product>().FirstOrDefaultAsync(
            predicate: x => x.Id == request.Id,
            includes: [ nameof(Product.ProductDetail),
                        nameof(Product.Category),
                        nameof(Product.FileAttachments),
                        $"{nameof(Product.ProductDetail)}.{nameof(ProductDetail.SizePrices)}"],
            asNoTracking: true
        );

            if (product == null)
            {
                return TResult<ProductDto>.Failure("Product not found", ErrorCodes.NOT_FOUND);
            }

            var productDto = product.Adapt<ProductDto>();

            return TResult<ProductDto>.Success(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get Product by ID Failed");
            return TResult<ProductDto>.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);  
        }
    }
}
