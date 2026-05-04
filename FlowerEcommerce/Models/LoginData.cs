namespace FlowerEcommerce.View.Models;

public class LoginData
{
    public TokenModel? TokenModel { get; set; }
    public ulong UserId { get; set; }
    public string? UserName { get; set; }
    public string? AllowedRole { get; set; }

}
