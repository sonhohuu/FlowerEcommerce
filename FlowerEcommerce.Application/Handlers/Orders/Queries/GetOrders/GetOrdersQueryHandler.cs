namespace FlowerEcommerce.Application.Handlers.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, TResult<IPaginate<OrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetOrdersQueryHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    private readonly IDateTimeService _dateTimeService;
    public GetOrdersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetOrdersQueryHandler> logger, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }
    public async Task<TResult<IPaginate<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Repository<Order>().GetPagingListAsync(
            predicate: o => (string.IsNullOrEmpty(request.SearchKeyword) 
                            || o.CustomerName.Contains(request.SearchKeyword) 
                            || o.OrderCode.Contains(request.SearchKeyword)) &&
                           (!request.IsSelf.HasValue || o.UserId == _currentUserService.UserId),
            selector: o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.CreatedAt.HasValue ? o.CreatedAt.Value.ToString("dd-MM-yyyy") : null,
                OrderCode = o.OrderCode,
                CustomerName = o.CustomerName,
                Address = o.Address,
                PhoneNumber = o.PhoneNumber,
                Note = o.Note,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                PaymentMethod = o.PaymentMethod.ToString(),
                CounterAccountBankName = o.Payment.CounterAccountBankName,
                Details = o.Items.Select(od => new OrderDetailDto
                {
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    Label = od.Label,
                    Price = od.Price,
                }).ToList()
            },
            page: request.Page,
            size: request.PageSize
        );
        return TResult<IPaginate<OrderDto>>.Success(orders);
    }
}
