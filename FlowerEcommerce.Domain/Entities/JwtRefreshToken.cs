namespace FlowerEcommerce.Domain.Entities;

public class JwtRefreshToken : BaseEntity
{
    public Guid RefreshToken { get; set; }
    public JwtRefreshTokenStatusEnum Status { get; set; } = JwtRefreshTokenStatusEnum.Active;
    public ulong UserId { get; set; }
    public DateTime Expires { get; set; }
}
