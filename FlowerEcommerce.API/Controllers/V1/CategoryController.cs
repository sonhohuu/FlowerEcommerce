using FlowerEcommerce.Application.Handlers.Categories.Commands.CreateCategory;
using FlowerEcommerce.Application.Handlers.Categories.Commands.DeleteCategory;
using FlowerEcommerce.Application.Handlers.Categories.Commands.UpdateCategory;
using FlowerEcommerce.Application.Handlers.Categories.Queries.GetCategories;

namespace FlowerEcommerce.API.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/[controller]")]
public class CategoryController : BaseController
{
    [Authorize(Policy = AppPolicy.AdminOnly)]
    [HttpPost(Name = "CreateCategory")]
    public async Task<IActionResult> CreateCategory(
    [FromBody] CreateCategoryCommand command,
    CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [Authorize(Policy = AppPolicy.AdminOnly)]
    [HttpPut("{id}", Name = "UpdateCategory")]
    public async Task<IActionResult> UpdateCategory(
        [FromBody] UpdateCategoryCommand command,
        [FromRoute] ulong id,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [Authorize(Policy = AppPolicy.AdminOnly)]
    [HttpDelete("{id}", Name = "DeleteCategory")]
    public async Task<IActionResult> DeleteCategory(
        [FromRoute] ulong id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [HttpGet(Name = "GetCategories")]
    public async Task<IActionResult> GetCategories(
        CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Data))
            : HandleResult(result);
    }
}
