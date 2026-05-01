namespace FlowerEcommerce.Application.Handlers.Categories.Commands.UpdateCategory;

[EnableUnitOfWork]
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingCategoryName = await _unitOfWork.Repository<Category>()
                .AnyAsync(predicate: c => c.Name == request.Name.Trim(),
                                    cancellationToken: cancellationToken);

            if (existingCategoryName)
            {
                _logger.LogWarning("Category {CategoryName} already exists", request.Name);
                return TResult.Failure(MessageKey.CategoryAlreadyExists, ErrorCodes.ALREADY_EXISTS);
            }

            var existingCategory = await _unitOfWork.Repository<Category>()
                .FirstOrDefaultAsync(predicate: c => c.Id == request.Id,
                                    cancellationToken: cancellationToken);

            if (existingCategory == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found", request.Id);
                return TResult.Failure(MessageKey.CategoryNotFound, ErrorCodes.NOT_FOUND);
            }

            existingCategory.Name = request.Name.Trim();
            existingCategory.Slug = StringUtils.GenerateSlug(request.Name);

            _unitOfWork.Repository<Category>().Update(existingCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated Category {CategoryName} Successfully", existingCategory.Name);

            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update Category Failed");
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
