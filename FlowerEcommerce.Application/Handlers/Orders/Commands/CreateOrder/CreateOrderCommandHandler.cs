namespace FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;

[EnableUnitOfWork]
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;
    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateOrderCommandHandler> logger, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingProductIds = await _unitOfWork.Repository<Product>()
                .AnyAsync(p => request.Items.Select(i => i.ProductId).Contains(p.Id), cancellationToken);

            if (!existingProductIds)
            {
                return TResult.Failure(MessageKey.OrderItemNotFound);
            }

            var order = request.Adapt<Order>();
            order.UserId = _currentUserService.UserId;
            order.OrderCode = OrderCodeGenerator.GenerateOrderCode(request.PaymentMethod.ToString(), request.CustomerName);
            order.TotalAmount = request.Items.Sum(x => x.Quantity * x.Price);

            _unitOfWork.Repository<Order>().Add(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created Order Successfully");

            return TResult.Success();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return TResult.Failure("An error occurred while creating the order.");
        }
    }
}

public static class OrderCodeGenerator
{
    public static string GenerateOrderCode(string orderType, string customerName)
    {
        var orderTypePrefix = Enum.TryParse<PaymentMethod>(orderType, out var paymentMethod)
        ? paymentMethod switch
        {
            PaymentMethod.COD => "COD",
            PaymentMethod.BankTransfer => "BT",
            _ => "NF"
        }
        : "NF";

        // Lấy chữ cái đầu của từng từ trong tên khách hàng
        var initials = string.Concat(
            customerName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => char.ToUpper(word[0]))
        );

        // Timestamp tính cả milliseconds: yyyyMMddHHmmssfff
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

        return $"{orderTypePrefix}-{initials}-{timestamp}";
    }
}


