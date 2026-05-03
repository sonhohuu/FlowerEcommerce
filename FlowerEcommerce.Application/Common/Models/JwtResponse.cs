namespace FlowerEcommerce.Application.Common.Models;

public class JwtResponse
{
    public string? AccessToken { get; set; }
    public Guid? RefreshToken { get; set; }
    public DateTime Issued { get; set; }
    public ulong RefreshTokenExpireIn { get; set; }
    public DateTime RefreshTokenExpires { get; set; }
    public ulong AccessTokenExpireIn { get; set; }
    public DateTime AccessTokenExpires { get; set; }
}
