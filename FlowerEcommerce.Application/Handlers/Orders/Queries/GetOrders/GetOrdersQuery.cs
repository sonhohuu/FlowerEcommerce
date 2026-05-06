namespace FlowerEcommerce.Application.Handlers.Orders.Queries.GetOrders;

public class GetOrdersQuery : PaginationRequest, IRequest<TResult<IPaginate<OrderDto>>>
{
    public string? SearchKeyword { get; set; }
    public bool? IsSelf { get; set; }
}

public class OrderDto
{
    public ulong Id { get; set; }
    public string? OrderDate { get; set; }
    public string? OrderCode { get; set; }
    public string? CustomerName { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Note { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string? CounterAccountBankName { get; set; }
    public List<OrderDetailDto> Details { get; set; } = [];
}


public class OrderDetailDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Label { get; set; }
    public decimal Price { get; set; }
}


