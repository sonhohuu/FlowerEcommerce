namespace FlowerEcommerce.Application.Handlers.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, TResult<IPaginate<ProductListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetProductsQueryHandler> _logger;
    public GetProductsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetProductsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<TResult<IPaginate<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _unitOfWork.Repository<Product>().GetPagingListAsync<ProductListDto>(
            predicate: p => (string.IsNullOrEmpty(request.SearchKeyword) || p.Name.Contains(request.SearchKeyword) || p.ProductDetail.Slug.Contains(request.SearchKeyword)) &&
                            (!request.CategoryId.HasValue || p.CategoryId == request.CategoryId),
            includes: [ nameof(Product.ProductDetail),
                        nameof(Product.FileAttachments)],
            page: request.Page,
            size: request.PageSize
        );

            return TResult<IPaginate<ProductListDto>>.Success(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get Products Failed");
            return TResult<IPaginate<ProductListDto>>.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
