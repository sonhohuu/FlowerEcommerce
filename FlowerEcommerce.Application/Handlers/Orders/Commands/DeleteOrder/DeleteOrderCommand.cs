namespace FlowerEcommerce.Application.Handlers.Orders.Commands.DeleteOrder;

public class DeleteOrderCommand : IRequest<TResult>
{
    [SwaggerIgnore]
    public ulong Id { get; set; }
}
