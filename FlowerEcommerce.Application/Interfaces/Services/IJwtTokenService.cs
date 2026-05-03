namespace FlowerEcommerce.Application.Interfaces.Services;

public interface IJwtTokenService
{
    public Task<TResult<JwtResponse>> GetJwtToken(ulong userId, CancellationToken cancellationToken = default);

    public Task<TResult<JwtResponse>> RefreshJwtToken(
        Guid oldRefreshToken,
        CancellationToken cancellationToken = default
    );

    public Task<JwtRefreshToken?> RevokeRefreshToken(Guid refreshToken, CancellationToken cancellationToken = default);
}
