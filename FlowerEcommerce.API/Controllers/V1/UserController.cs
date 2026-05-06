using FlowerEcommerce.Application.Handlers.Users.Queries;
using Microsoft.AspNetCore.Authorization;

namespace FlowerEcommerce.API.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/[controller]")]
public class UserController : BaseController
{
    [Authorize(Policy = AppPolicy.AdminOnly)]
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query,
    CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Data))
            : HandleResult(result);
    }
}
