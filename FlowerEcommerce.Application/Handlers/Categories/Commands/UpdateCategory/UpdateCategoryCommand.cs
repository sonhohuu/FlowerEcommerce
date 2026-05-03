using Swashbuckle.AspNetCore.Annotations;

namespace FlowerEcommerce.Application.Handlers.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand : IRequest<TResult>
{
    [SwaggerIgnore]
    public ulong Id { get; set; }
    public required string Name { get; init; }
}
