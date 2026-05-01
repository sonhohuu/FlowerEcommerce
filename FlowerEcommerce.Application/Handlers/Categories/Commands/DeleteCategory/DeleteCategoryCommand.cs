namespace FlowerEcommerce.Application.Handlers.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand : IRequest<TResult>
{
    public required ulong Id { get; init; }
}
