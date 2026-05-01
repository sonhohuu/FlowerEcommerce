namespace FlowerEcommerce.Application.Handlers.Categories.Commands.CreateCategory;

public record CreateCategoryCommand : IRequest<TResult>
{
    public required string Name { get; init; } = null!;
}
