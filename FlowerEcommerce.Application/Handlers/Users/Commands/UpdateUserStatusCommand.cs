namespace FlowerEcommerce.Application.Handlers.Users.Commands;

public class UpdateUserStatusCommand : IRequest<TResult>
{
    [SwaggerIgnore]
    public ulong UserId { get; set; }
    public UserStatusEnum Status { get; set; }
}
