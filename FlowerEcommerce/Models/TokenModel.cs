namespace FlowerEcommerce.View.Models;

public class TokenModel
{
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? AccessTokenExpires { get; set; }
    public DateTime? RefreshTokenExpires { get; set; }
}
