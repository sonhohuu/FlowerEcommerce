using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.Application.Handlers.Auth;

public class RefreshTokenCommand : IRequest<TResult<JwtResponse>>
{
    [Required] public required Guid RefreshToken { get; set; }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TResult<JwtResponse>>
{
    private readonly IJwtTokenService jwtTokenService;
    public RefreshTokenCommandHandler(IJwtTokenService jwtTokenService)
    {
        this.jwtTokenService = jwtTokenService;
    }

    public async Task<TResult<JwtResponse>> Handle(
        RefreshTokenCommand request, CancellationToken cancellationToken
    )
    {
        var jwtTokenResponse = await jwtTokenService.RefreshJwtToken(request.RefreshToken, cancellationToken);
        return jwtTokenResponse;
    }
}
