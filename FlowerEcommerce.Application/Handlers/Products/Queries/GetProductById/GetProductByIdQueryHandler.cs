namespace FlowerEcommerce.Application.Handlers.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, TResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<TResult<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().FirstOrDefaultAsync(
            predicate: x => x.Id == request.Id,
            includes: [nameof(Product.Category)],
            asNoTracking: true
        );

        if (product == null)
        {
            return TResult<ProductDto>.Failure("Product not found", ErrorCodes.NOT_FOUND);
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryName = product.Category?.Name
        };
        return TResult<ProductDto>.Success(productDto);
    }
}
