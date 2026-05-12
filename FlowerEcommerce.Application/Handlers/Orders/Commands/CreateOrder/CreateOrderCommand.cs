namespace FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<TResult<CreateOrderResult>>
{
    public string CustomerName { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string? Note { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public List<OrderItemDto> Items { get; init; } = [];
}

public record OrderItemDto
{
    public ulong ProductId { get; init; }
    public int Quantity { get; init; }
    public string Label { get; init; } = string.Empty;
    public decimal Price { get; init; }

}

public record CreateOrderResult(
    ulong OrderId,
    string OrderCode,
    string? CheckoutUrl,  // null nếu COD
    string? QrCode        // null nếu COD
);
