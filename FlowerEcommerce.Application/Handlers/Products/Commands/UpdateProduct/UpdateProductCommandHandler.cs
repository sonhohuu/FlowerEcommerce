namespace FlowerEcommerce.Application.Handlers.Products.Commands.UpdateProduct;

[EnableUnitOfWork]
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingProduct = await _unitOfWork.Repository<Product>()
                .FirstOrDefaultAsync(predicate:x => x.Id == request.Id);

            if (existingProduct == null)
            {
                return TResult.Failure(MessageKey.ProductNotFound, ErrorCodes.NOT_FOUND);
            }

            var existingCategory = await _unitOfWork.Repository<Category>()
                .AnyAsync(c => c.Id == request.CategoryId);

            if (existingCategory)
            {
                return TResult.Failure(MessageKey.CategoryNotFound, ErrorCodes.NOT_FOUND);
            }

            existingProduct.Name = request.Name ?? existingProduct.Name;
            existingProduct.Description = request.Description ?? existingProduct.Description;
            existingProduct.Price = request.Price ?? existingProduct.Price;
            existingProduct.CategoryId = request.CategoryId ?? existingProduct.CategoryId;

            _unitOfWork.Repository<Product>().Update(existingProduct);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated Product {ProductName} Successfully", existingProduct.Name);

            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update Product Failed");
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
