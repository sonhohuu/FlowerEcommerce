namespace FlowerEcommerce.Application.Handlers.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingProduct = await _unitOfWork.Repository<Product>()
                .FirstOrDefaultAsync(predicate: x => x.Id == request.Id);

            if (existingProduct == null)
            {
                return TResult.Failure(MessageKey.ProductNotFound, ErrorCodes.NOT_FOUND);
            }

            _unitOfWork.Repository<Product>().Remove(existingProduct);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted Product {ProductName} Successfully", existingProduct.Name);

            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete Product Failed");
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
