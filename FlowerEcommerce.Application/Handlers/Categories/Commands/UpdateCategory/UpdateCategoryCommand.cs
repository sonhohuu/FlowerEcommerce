namespace FlowerEcommerce.Application.Handlers.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand : IRequest<TResult>
{
    public required ulong Id { get; init; }
    public required string Name { get; init; }
}
