namespace FlowerEcommerce.Application.Handlers.Categories.Commands.DeleteCategory;

[EnableUnitOfWork]
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;
    public DeleteCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<TResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingCategory = await _unitOfWork.Repository<Category>()
                .FirstOrDefaultAsync(predicate: x => x.Id == request.Id);
            if (existingCategory == null)
            {
                return TResult.Failure(MessageKey.CategoryNotFound, ErrorCodes.NOT_FOUND);
            }
            _unitOfWork.Repository<Category>().Remove(existingCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Deleted Category {CategoryName} Successfully", existingCategory.Name);
            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete Category Failed");
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
