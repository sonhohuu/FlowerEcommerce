namespace FlowerEcommerce.Domain.Entities;
public class Order : ModificationAuditedEntity
{
    public string OrderCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Note { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Confirming;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
    public ApplicationUser? User { get; set; }
    public ulong? UserId { get; set; }
    public IList<OrderItem> Items { get; set; } = [];
    public OrderPayment? Payment { get; set; }
}
