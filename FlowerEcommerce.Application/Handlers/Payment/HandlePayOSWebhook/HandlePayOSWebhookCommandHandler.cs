namespace FlowerEcommerce.Application.Handlers.Payment.HandlePayOSWebhook;

public class HandlePayOSWebhookHandler : IRequestHandler<HandlePayOSWebhookCommand, TResult<bool>>
{
    private readonly IPayOSService _payOS;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<HandlePayOSWebhookHandler> _logger;

    public HandlePayOSWebhookHandler(
        IPayOSService payOS,
        IUnitOfWork unitOfWork,
        ILogger<HandlePayOSWebhookHandler> logger)
    {
        _payOS = payOS;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult<bool>> Handle(HandlePayOSWebhookCommand command, CancellationToken ct)
    {
        var webhook = command.Data;

        // 1. Verify chữ ký
        if (!_payOS.VerifyWebhookData(webhook))
        {
            _logger.LogWarning("PayOS webhook: invalid signature");
            return TResult<bool>.Failure("Invalid signature");
        }

        // 2. Chỉ xử lý khi thanh toán thành công
        // code "00" = thành công theo PayOS docs
        if (webhook.Code != "00" || webhook.Data is null)
        {
            _logger.LogInformation("PayOS webhook: non-success code {Code}", webhook.Code);
            return TResult<bool>.Success(true); // vẫn trả 200 để PayOS không retry
        }

        var orderCode = webhook.Data.OrderCode;

        // 3. Tìm order
        var order = await _unitOfWork.Repository<Order>().FirstOrDefaultAsync(
            predicate: o => o.OrderCode == orderCode.ToString(), 
            cancellationToken: ct);
        if (order is null)
        {
            _logger.LogWarning("PayOS webhook: order not found for code {Code}", orderCode);
            return TResult<bool>.Failure("Order not found");
        }

        // 4. Idempotent — bỏ qua nếu đã xử lý rồi
        if (order.Status == OrderStatus.Processing)
        {
            _logger.LogInformation("PayOS webhook: order {Id} already processed", order.Id);
            return TResult<bool>.Success(true);
        }

        // 5. Cập nhật status → Processing
        order.Status = OrderStatus.Processing;
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "PayOS webhook: order {Id} updated to Processing (amount: {Amount})",
            order.Id, webhook.Data.Amount);

        return TResult<bool>.Success(true);
    }
}
