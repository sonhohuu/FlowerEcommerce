namespace FlowerEcommerce.Application.Interfaces.Services;

public interface ICurrentUserService
{
    public ulong? UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool IsAuthenticated { get; set; }
    public List<ulong> RoleIds { get; set; }
    public bool IsAdmin { get; }
}
