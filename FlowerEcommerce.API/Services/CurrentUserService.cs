namespace FlowerEcommerce.API.Services;
public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor? httpContextAccessor)
    {
        UserName = string.Empty;
        Email = string.Empty;

        var httpContext = httpContextAccessor?.HttpContext;
        if (httpContext?.User != null &&
            httpContext.User.Identity != null &&
            httpContext.User.Identity!.IsAuthenticated)
            FillCurrentUserInfoData(httpContext.User);

        RoleIds ??= [];
    }

    public ulong? UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool IsAuthenticated { get; set; }
    public List<ulong> RoleIds { get; set; }

    public bool IsAdmin =>
        RoleIds.Contains((int)AppRoleEnum.SuperAdministrator) ||
        RoleIds.Contains((int)AppRoleEnum.Administrator);

    private void FillCurrentUserInfoData(ClaimsPrincipal claimsPrincipal)
    {
        if (ulong.TryParse(claimsPrincipal.FindFirstValue(AppClaimTypes.UserId), out var userId)) UserId = userId;

        UserName = claimsPrincipal.FindFirstValue(AppClaimTypes.UserName)!;
        Email = claimsPrincipal.FindFirstValue(AppClaimTypes.Email)!;

        RoleIds = JsonSerializer.Deserialize<List<ulong>>(
            claimsPrincipal.FindFirstValue(AppClaimTypes.RoleIds)!,
            AppConstants.JsonSerializerOptions
        )!.ToList();

        IsAuthenticated = true;
    }
}
