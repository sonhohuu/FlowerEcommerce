using FlowerEcommerce.Application.Handlers.Payment.CreatePaymentLink;

namespace FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;

[EnableUnitOfWork]
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, TResult<CreateOrderResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public CreateOrderCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateOrderCommandHandler> logger,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<TResult<CreateOrderResult>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate products
            var existingProductIds = await _unitOfWork.Repository<Product>()
                .AnyAsync(p => request.Items.Select(i => i.ProductId).Contains(p.Id), cancellationToken);

            if (!existingProductIds)
                return TResult<CreateOrderResult>.Failure(MessageKey.OrderItemNotFound);

            // 2. Tạo Order
            var order = request.Adapt<Order>();
            order.UserId = _currentUserService.UserId;
            order.OrderCode = OrderCodeGenerator.GenerateOrderCode(request.PaymentMethod.ToString(), request.CustomerName);
            order.TotalAmount = request.Items.Sum(x => x.Quantity * x.Price);

            _unitOfWork.Repository<Order>().Add(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created Order {OrderId} successfully", order.Id);

            // 3. Nếu BankTransfer → tạo PayOS link qua MediatR
            if (request.PaymentMethod == PaymentMethod.BankTransfer)
            {
                var paymentResult = await _mediator.Send(
                    new CreatePaymentLinkCommand { OrderId = order.Id },
                    cancellationToken);

                if (!paymentResult.IsSuccess)
                {
                    _logger.LogWarning("Order {OrderId} created but PayOS link failed", order.Id);
                }

                return TResult<CreateOrderResult>.Success(new CreateOrderResult(
                    OrderId: order.Id,
                    OrderCode: order.OrderCode,
                    CheckoutUrl: paymentResult.Data?.CheckoutUrl,
                    QrCode: paymentResult.Data?.QrCode
                ));
            }

            // 4. COD → không cần link
            return TResult<CreateOrderResult>.Success(new CreateOrderResult(
                OrderId: order.Id,
                OrderCode: order.OrderCode,
                CheckoutUrl: null,
                QrCode: null
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return TResult<CreateOrderResult>.Failure("An error occurred while creating the order.");
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
            PaymentMethod.BankTransfer => "BTF",
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


