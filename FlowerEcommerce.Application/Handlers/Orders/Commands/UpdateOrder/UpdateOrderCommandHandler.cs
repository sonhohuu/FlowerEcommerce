using FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;

namespace FlowerEcommerce.Application.Handlers.Orders.Commands.UpdateOrder;

[EnableUnitOfWork]
public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;
    public UpdateOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingOrder = await _unitOfWork.Repository<Order>().FirstOrDefaultAsync(
                predicate: o => o.Id == request.Id,
                includes: [nameof(Order.Items)],
                cancellationToken: cancellationToken);

            if (existingOrder == null)
            {
                return TResult.Failure(MessageKey.OrderNotFound);
            }

            // Terminal states: không thể update
            if (existingOrder.Status is OrderStatus.Success or OrderStatus.Failed)
                return TResult.Failure(MessageKey.OrderCannotBeUpdated);

            // Validate status transition: chỉ được tiến theo thứ tự, không được lùi
            if (request.Status < existingOrder.Status)
                return TResult.Failure(MessageKey.InvalidStatusTransition);

            // Chỉ update các fields khi existingOrder đang ở Confirming
            if (existingOrder.Status == OrderStatus.Confirming)
            {
                // Xử lý Items trước khi Adapt để tránh conflict
                if (request.Items is not null && request.Items.Count > 0)
                {
                    var existingProductIds = await _unitOfWork.Repository<Product>()
                        .AnyAsync(p => request.Items.Select(i => i.ProductId).Contains(p.Id), cancellationToken);

                    if (!existingProductIds)
                    {
                        return TResult.Failure(MessageKey.OrderItemNotFound);
                    }

                    _unitOfWork.Repository<OrderItem>().RemoveRange(existingOrder.Items);
                    existingOrder.Items.Clear();
                }

                request.Adapt(existingOrder);
                existingOrder.TotalAmount = existingOrder.Items.Sum(oi => oi.Quantity * oi.Price);
                existingOrder.OrderCode = OrderCodeGenerator.GenerateOrderCode(existingOrder.PaymentMethod.ToString(), existingOrder.CustomerName);
            }

            existingOrder.Status = request.Status;

            _unitOfWork.Repository<Order>().Update(existingOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated Order Successfully");
            return TResult.Success();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order");
            return TResult.Failure("An error occurred while updating the order.");
        }
    }
}
