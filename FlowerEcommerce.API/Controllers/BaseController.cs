namespace FlowerEcommerce.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
//TODO: route config 
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    // HandleResult methods
    protected IActionResult HandleResult<T>(TResult<T> result)
    {
        if (result.IsSuccess && result.Data != null)
            return Ok(ApiResponse<T>.Ok(result.Data));

        if (result.ValidationErrors != null && result.ValidationErrors.Any())
            return BadRequest(ApiResponse<T>.Fail("Validation failed", ErrorCodes.BAD_REQUEST, result.ValidationErrors));

        return result.ErrorCode switch
        {
            ErrorCodes.NOT_FOUND => NotFound(ApiResponse<T>.Fail(result.Error!, result.ErrorCode)),
            ErrorCodes.UNAUTHORIZED => Unauthorized(ApiResponse<T>.Fail(result.Error!, result.ErrorCode)),
            ErrorCodes.FORBIDDEN => StatusCode(StatusCodes.Status403Forbidden, ApiResponse<T>.Fail(result.Error!, result.ErrorCode)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error!, result.ErrorCode))
        };
    }

    protected IActionResult HandleResult(TResult result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<object>.Ok(null, "Operation completed successfully"));

        return HandleResult(TResult<object>.Failure(result.Error!, result.ErrorCode));
    }
}
