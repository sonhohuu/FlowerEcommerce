namespace FlowerEcommerce.Domain.Entities;

public class OrderPayment : ModificationAuditedEntity
{
    public PaymentMethod Method { get; set; }
    public long? PayOsOrderCode { get; set; }       // số gửi lên PayOS
    public string? PaymentLinkId { get; set; }      // ID link PayOS trả về
    public string? PaymentUrl { get; set; }         // URL checkout cho khách
    public string? TransactionId { get; set; }      // mã GD sau khi PAID
    public PayOsPaymentStatus? PayOsStatus { get; set; }
    public DateTime? PaidAt { get; set; }

    // Thông tin tài khoản người chuyển (PayOS webhook trả về)
    public string? CounterAccountName { get; set; }
    public string? CounterAccountNumber { get; set; }
    public string? CounterAccountBankName { get; set; }

    public ulong OrderId { get; set; }
    public Order Order { get; set; }
}
