namespace FlowerEcommerce.Application.Handlers.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, TResult<List<CategoryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<TResult<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Repository<Category>()
                .GetAllAsync(cancellationToken: cancellationToken);

        var categoryDtos = categories.Adapt<List<CategoryDto>>();
        return TResult<List<CategoryDto>>.Success(categoryDtos);
    }
}
