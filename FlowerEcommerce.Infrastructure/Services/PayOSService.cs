using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using AppPayOSOptions = FlowerEcommerce.Application.Common.Configs.PayOSOptions; // alias tránh conflict

namespace FlowerEcommerce.Infrastructure.Services;

public class PayOSService : IPayOSService
{
    private readonly PayOSClient _client;
    private readonly AppPayOSOptions _options;

    public PayOSService(IOptions<AppPayOSOptions> options)
    {
        _options = options.Value;
        _client = new PayOSClient(new PayOSOptions
        {
            ClientId = _options.ClientId,
            ApiKey = _options.ApiKey,
            ChecksumKey = _options.ChecksumKey
        });
    }

    // ── Tạo link thanh toán ──────────────────────────────────────────────────
    public async Task<CreatePaymentResult> CreatePaymentLinkAsync(CreatePaymentRequest req)
    {
        var paymentRequest = new CreatePaymentLinkRequest
        {
            OrderCode = req.OrderCode,
            Amount = req.Amount,
            Description = "Thanh toán hóa đơn",
            ReturnUrl = req.ReturnUrl ?? _options.ReturnUrl,   // fallback về config nếu caller không truyền
            CancelUrl = req.CancelUrl ?? _options.CancelUrl,
            BuyerName = req.BuyerName,
            BuyerPhone = req.BuyerPhone,
            Items = req.Items
                .Select(i => new PaymentLinkItem
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                })
                .ToList()
        };

        var result = await _client.PaymentRequests.CreateAsync(paymentRequest);

        return new CreatePaymentResult(
            CheckoutUrl: result.CheckoutUrl,
            QrCode: result.QrCode ?? "",
            PaymentLinkId: result.PaymentLinkId,
            OrderCode: result.OrderCode
        );
    }

    // ── Lấy thông tin link ───────────────────────────────────────────────────
    public async Task<PaymentLinkInfo> GetPaymentLinkInfoAsync(long orderCode)
    {
        var info = await _client.PaymentRequests.GetAsync(orderCode);

        return new PaymentLinkInfo(
            OrderCode: info.OrderCode,
            Status: info.Status.ToString()  ,
            Amount: (int)info.Amount,
            Currency: /*info.Currency ?? */ "VND"
        );
    }

    // ── Huỷ link thanh toán ──────────────────────────────────────────────────
    public async Task<PaymentLinkInfo> CancelPaymentLinkAsync(long orderCode, string? reason = null)
    {
        var info = await _client.PaymentRequests.CancelAsync(orderCode, reason);

        return new PaymentLinkInfo(
            OrderCode: info.OrderCode,
            Status: info.Status.ToString(),
            Amount: (int)info.Amount,
            Currency: /*info.Currency ?? */"VND"
        );
    }

    // ── Verify webhook signature ─────────────────────────────────────────────
    public bool VerifyWebhookData(Application.Interfaces.Services.WebhookData data)
    {
        try
        {
            var webhook = new Webhook
            {
                Code = data.Code,
                Description = data.Desc,
                Success = data.Success,
                Signature = data.Signature,
                Data = data.Data is null ? null : new PayOS.Models.Webhooks.WebhookData
                {
                    OrderCode = data.Data.OrderCode,
                    Amount = data.Data.Amount,
                    Description = data.Data.Description,
                    AccountNumber = data.Data.AccountNumber,
                    Reference = data.Data.Reference,
                    TransactionDateTime = data.Data.TransactionDateTime,
                    Currency = data.Data.Currency,
                    PaymentLinkId = data.Data.PaymentLinkId,
                    Code = data.Data.Code,
                    Description2 = data.Data.Desc,
                }
            };

            _client.Webhooks.VerifyAsync(webhook).GetAwaiter().GetResult();
            return true;
        }
        catch
        {
            return false;
        }
    }
}