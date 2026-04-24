namespace FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

[EnableUnitOfWork]
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingProduct = await _unitOfWork.Repository<Product>()
                .AnyAsync(predicate: p => p.Name == request.Name.Trim());

            if (existingProduct)
            {
                return TResult.Failure(MessageKey.ProductAlreadyExists, ErrorCodes.ALREADY_EXISTS);
            }

            var existingCategory = await _unitOfWork.Repository<Category>()
                .AnyAsync(predicate: c => c.Id == request.CategoryId);

            if (existingCategory)
            {
                return TResult.Failure(MessageKey.CategoryNotFound, ErrorCodes.NOT_FOUND);
            }

            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId
            };

            _unitOfWork.Repository<Product>().Add(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created Product {ProductName} Successfully", product.Name);

            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create Product Failed");
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
