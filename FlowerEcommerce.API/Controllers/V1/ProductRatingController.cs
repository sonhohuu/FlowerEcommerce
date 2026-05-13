using FlowerEcommerce.Application.Handlers.ProductRatings.Commands.DeleteProductRating;
using FlowerEcommerce.Application.Handlers.ProductRatings.Commands.UpsertProductRating;
using FlowerEcommerce.Application.Handlers.ProductRatings.Queries.GetProductRatings;


namespace FlowerEcommerce.API.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/[controller]")]
public class ProductRatingController : BaseController
{
    [Authorize(Policy = AppPolicy.CustomerOnly)]
    [HttpPost(Name = "UpsertProductRating")]
    public async Task<IActionResult> UpsertProductRating    (
    [FromBody] UpsertProductRatingCommand command,
    CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [Authorize(Policy = AppPolicy.AdminOrCustomer)]
    [HttpDelete("{id}", Name = "DeleteProductRating")]
    public async Task<IActionResult> DeleteProductRating(
        [FromRoute] ulong id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductRatingCommand { Id = id };
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [HttpGet(Name = "GetProductRatings")]
    public async Task<IActionResult> GetProductRatings(
        [FromQuery] GetProductRatingsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Data))
            : HandleResult(result);
    }
}
