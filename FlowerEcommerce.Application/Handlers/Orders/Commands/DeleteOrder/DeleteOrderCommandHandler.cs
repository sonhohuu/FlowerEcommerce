namespace FlowerEcommerce.Application.Handlers.Orders.Commands.DeleteOrder;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteOrderCommandHandler> _logger;
    public DeleteOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<TResult> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingOrder = await _unitOfWork.Repository<Order>().FirstOrDefaultAsync(
                predicate: o => o.Id == request.Id,
                cancellationToken: cancellationToken);
            if (existingOrder == null)
            {
                return TResult.Failure(MessageKey.OrderNotFound);
            }

            if(existingOrder.Status != OrderStatus.Confirming)
            {
                return TResult.Failure(MessageKey.OrderCannotBeDeleted);
            }

            existingOrder.Status = OrderStatus.Failed;

            _unitOfWork.Repository<Order>().Update(existingOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Changed Order {OrderId} to Failed Successfully", request.Id);
            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing order status {OrderId}", request.Id);
            return TResult.Failure("An error occurred while changing the order status.");
        }
    }
}
