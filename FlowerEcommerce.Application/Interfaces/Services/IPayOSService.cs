

namespace FlowerEcommerce.Application.Interfaces.Services;

public interface IPayOSService
{
    Task<CreatePaymentResult> CreatePaymentLinkAsync(CreatePaymentRequest request);
    Task<PaymentLinkInfo> GetPaymentLinkInfoAsync(long orderCode);
    Task<PaymentLinkInfo> CancelPaymentLinkAsync(long orderCode, string? reason = null);
    bool VerifyWebhookData(WebhookData data);
}

public record CreatePaymentRequest(
    long OrderCode,
    int Amount,
    string Description,
    string BuyerName,
    string BuyerPhone,
    List<PaymentItem> Items,
    string ReturnUrl,
    string CancelUrl
);

public record PaymentItem
{
    public string Name { get; init; } = default!;
    public int Quantity { get; init; }
    public int Price { get; init; }
}

public record CreatePaymentResult(
    string CheckoutUrl,
    string QrCode,
    string PaymentLinkId,
    long OrderCode
);

public record PaymentLinkInfo(
    long OrderCode,
    string Status,       // PENDING | PAID | CANCELLED | EXPIRED
    int Amount,
    string Currency
);

public record WebhookData(
    string Code,
    string Desc,
    bool Success,
    WebhookDataPayload? Data,
    string Signature
);

public record WebhookDataPayload(
    long OrderCode,
    int Amount,
    string Description,
    string AccountNumber,
    string Reference,
    string TransactionDateTime,
    string Currency,
    string PaymentLinkId,
    string Code,
    string Desc
);
