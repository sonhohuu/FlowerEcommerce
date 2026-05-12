using FlowerEcommerce.Application.Common.Configs;
using FlowerEcommerce.Application.Common.Models;
using FlowerEcommerce.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FlowerEcommerce.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeService _dateTimeService;
    private readonly JwtBearerConfig _jwtBearerConfig;
    private readonly UserManager<ApplicationUser> _userManager;
    public JwtTokenService(IUnitOfWork unitOfWork, IDateTimeService dateTimeService, IOptions<JwtBearerConfig> jwtBearerConfigOptions, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _dateTimeService = dateTimeService;
        _jwtBearerConfig = jwtBearerConfigOptions.Value;
        _userManager = userManager;
    }

    public async Task<TResult<JwtResponse>> GetJwtToken(ulong userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return TResult<JwtResponse>.Failure("Cannot find user.");
        if (user.Status != UserStatusEnum.Active) return TResult<JwtResponse>.Failure("User was inactive.");

        var refreshToken = Guid.NewGuid();
        List<Claim> claims =
        [
            new(AppClaimTypes.UserId, user.Id.ToString()),
            new(AppClaimTypes.UserName, user.UserName ?? string.Empty),
            new(AppClaimTypes.Email, user.Email ?? string.Empty),
            new(AppClaimTypes.RefreshToken, refreshToken.ToString()),
            new(AppClaimTypes.RoleIds, user.Role.ToString())
        ];

        var now = _dateTimeService.UtcNow;
        var refreshTokenExpires = now.AddSeconds(_jwtBearerConfig.App!.RefreshTokenExpiryTime);
        var accessTokenExpires = now.AddSeconds(_jwtBearerConfig.App!.AccessTokenExpiryTime);

        var jwt = new JwtSecurityToken(
            claims: claims,
            notBefore: now,
            expires: accessTokenExpires,
            issuer: _jwtBearerConfig.App!.Issuer,
            audience: _jwtBearerConfig.App!.Audience,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtBearerConfig.App!.SecretKey)),
                SecurityAlgorithms.HmacSha256
            )
        );

        await SaveAndCacheToken(user.Id, refreshToken, refreshTokenExpires, cancellationToken);

        return TResult<JwtResponse>.Success(new JwtResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            RefreshToken = refreshToken,
            Issued = now,
            RefreshTokenExpireIn = _jwtBearerConfig.App!.RefreshTokenExpiryTime,
            RefreshTokenExpires = refreshTokenExpires,
            AccessTokenExpireIn = _jwtBearerConfig.App!.AccessTokenExpiryTime,
            AccessTokenExpires = accessTokenExpires
        });
    }

    public async Task<TResult<JwtResponse>> RefreshJwtToken(Guid oldRefreshToken,
    CancellationToken cancellationToken = default)
    {
        var jwtRefreshToken = await RevokeRefreshToken(oldRefreshToken, cancellationToken);

        if (jwtRefreshToken == null) return TResult<JwtResponse>.Failure("Invalid Refresh Token.");

        var jwtToken = await GetJwtToken(jwtRefreshToken.UserId, cancellationToken);
        return jwtToken;
    }

    public async Task<JwtRefreshToken?> RevokeRefreshToken(Guid refreshToken,
        CancellationToken cancellationToken = default)
    {
        var jwtRevokeToken = await _unitOfWork.Repository<JwtRefreshToken>().FirstOrDefaultAsync(
            predicate: x => x.RefreshToken == refreshToken &&
                 x.Status == JwtRefreshTokenStatusEnum.Active &&
                 x.Expires > _dateTimeService.UtcNow,
            orderBy: q => q.OrderByDescending(x => x.Expires),
            cancellationToken: cancellationToken
        );

        if (jwtRevokeToken == null) return null;

        //revoke the old token from db
        jwtRevokeToken.Status = JwtRefreshTokenStatusEnum.Revoked;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return jwtRevokeToken;
    }

    private async Task SaveAndCacheToken(
        ulong userId, Guid refreshToken, DateTime refreshTokenExpires,
        CancellationToken cancellationToken = default
    )
    {
        //refreshTokenCachingService.AddRefreshTokenToCache(refreshToken, JwtRefreshTokenStatusEnum.Active,
        //    refreshTokenExpires);
        _unitOfWork.Repository<JwtRefreshToken>().Add(new JwtRefreshToken
        {
            RefreshToken = refreshToken,
            Status = JwtRefreshTokenStatusEnum.Active,
            Expires = refreshTokenExpires,
            UserId = userId
        });
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
