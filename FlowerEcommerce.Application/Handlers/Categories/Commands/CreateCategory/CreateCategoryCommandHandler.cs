namespace FlowerEcommerce.Application.Handlers.Categories.Commands.CreateCategory;

[EnableUnitOfWork]
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingCategory = await _unitOfWork.Repository<Category>()
                .AnyAsync(predicate: c => c.Name == request.Name.Trim(), 
                                    cancellationToken: cancellationToken);

            if (existingCategory)
            {
                _logger.LogWarning("Category {CategoryName} already exists", request.Name);
                return TResult.Failure(MessageKey.CategoryAlreadyExists, ErrorCodes.ALREADY_EXISTS);
            }

            var category = new Category
            {
                Name = request.Name.Trim(),
                Slug = StringUtils.GenerateSlug(request.Name)
            };

            _unitOfWork.Repository<Category>().Add(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created Category {CategoryName} Successfully", category.Name);

            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create Category Failed");
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
