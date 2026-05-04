namespace FlowerEcommerce.Application.Handlers.Users.Queries;

public class GetUsersQuery : PaginationRequest, IRequest<TResult<IPaginate<UserDto>>>
{
    public string? SearchKeyword { get; set; }
}

public class UserDto
{
    public ulong Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
    public string Status { get; set; } = null!;
}
