using FlowerEcommerce.Application.Handlers.Auth;

namespace FlowerEcommerce.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseController
{
    [HttpPost("login", Name = "AuthLogin")]
    [ProducesResponseType(typeof(TResult<LoginResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Data))
            : HandleResult(result);
    }

    [HttpPost("refresh-token", Name = "AuthRefreshToken")]
    [ProducesResponseType(typeof(TResult<JwtResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Data))
            : HandleResult(result);
    }

    [HttpPost("register", Name = "AuthRegister")]
    [ProducesResponseType(typeof(TResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }
}
