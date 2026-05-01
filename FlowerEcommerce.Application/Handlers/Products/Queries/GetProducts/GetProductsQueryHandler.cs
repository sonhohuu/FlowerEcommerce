namespace FlowerEcommerce.Application.Handlers.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, TResult<IPaginate<ProductListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<TResult<IPaginate<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
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
}
