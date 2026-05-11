namespace FlowerEcommerce.Application.Handlers.Users.Commands;

public class UpdateUserStatusCommand : IRequest<TResult>
{
    public required ulong UserId { get; set; }
    public UserStatusEnum Status { get; set; }
}
