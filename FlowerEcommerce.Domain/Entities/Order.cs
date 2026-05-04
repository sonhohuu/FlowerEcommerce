namespace FlowerEcommerce.Domain.Entities;
public class Order : DeletionAuditedEntity
{
    public DateTime? OrderDate { get; set; }
    public string OrderCode { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public ApplicationUser? User { get; set; }
    public ulong? UserId { get; set; }
    public IList<OrderItem> Items { get; set; } = [];
    public OrderPayment? Payment { get; set; }
}
