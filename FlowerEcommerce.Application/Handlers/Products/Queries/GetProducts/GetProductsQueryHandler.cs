namespace FlowerEcommerce.Application.Handlers.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, TResult<IPaginate<ProductDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<TResult<IPaginate<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Repository<Product>().GetPagingListAsync<ProductDto>(
            predicate: p => string.IsNullOrEmpty(request.SearchKeyword) || p.Name.Contains(request.SearchKeyword),
            includes: [nameof(Product.Category)],
            page: request.Page,
            size: request.PageSize
        );

        return TResult<IPaginate<ProductDto>>.Success(products);
    }
}
