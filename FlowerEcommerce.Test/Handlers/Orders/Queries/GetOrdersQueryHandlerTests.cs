using FlowerEcommerce.Application.Handlers.Orders.Queries.GetOrders;

namespace FlowerEcommerce.Test.Handlers.Orders.Queries;

public class GetOrdersQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly Mock<IDateTimeService> _dateTimeService;
    private readonly ILogger<GetOrdersQueryHandler> _logger;
    private readonly GetOrdersQueryHandler _handler;

    private readonly Mock<IBaseRepository<Order>> _orderRepo;

    public GetOrdersQueryHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _currentUserService = new Mock<ICurrentUserService>();
        _dateTimeService = new Mock<IDateTimeService>();
        _logger = NullLogger<GetOrdersQueryHandler>.Instance;

        _orderRepo = new Mock<IBaseRepository<Order>>();
        _unitOfWork.Setup(u => u.Repository<Order>()).Returns(_orderRepo.Object);
        _currentUserService.Setup(s => s.UserId).Returns(1);

        _handler = new GetOrdersQueryHandler(
            _unitOfWork.Object,
            _logger,
            _currentUserService.Object,
            _dateTimeService.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static GetOrdersQuery BuildQuery(
        string? searchKeyword = null,
        bool? isSelf = null,
        OrderStatus? status = null,
        int page = 1,
        int pageSize = 10) =>
        new GetOrdersQuery
        {
            SearchKeyword = searchKeyword,
            IsSelf = isSelf,
            Status = status,
            Page = page,
            PageSize = pageSize
        };

    private void SetupGetPagingList(IPaginate<OrderDto> returns) =>
    _orderRepo
        .Setup(r => r.GetPagingListAsync(
            It.IsAny<Expression<Func<Order, OrderDto>>>(),  // selector
            It.IsAny<Expression<Func<Order, bool>>>(),      // predicate
            It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),  // orderBy
            It.IsAny<List<string>>(),                       // includes
            It.IsAny<int>(),                                // page
            It.IsAny<int>(),                                // size
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    private static IPaginate<OrderDto> FakePaginate(
        List<OrderDto> items,
        int page = 1,
        int size = 10,
        int total = 0,
        int totalPages = 1)
    {
        var mock = new Mock<IPaginate<OrderDto>>();
        mock.Setup(p => p.Items).Returns(items);
        mock.Setup(p => p.Page).Returns(page);
        mock.Setup(p => p.Size).Returns(size);
        mock.Setup(p => p.Total).Returns(total == 0 ? items.Count : total);
        mock.Setup(p => p.TotalPages).Returns(totalPages);
        return mock.Object;
    }

    private static OrderDto FakeOrderDto(
        ulong id = 1,
        string orderCode = "COD-NVA-20240101",
        string status = "Confirming") =>
        new OrderDto
        {
            Id = id,
            OrderCode = orderCode,
            CustomerName = "Nguyen Van A",
            Status = status,
            TotalAmount = 100_000
        };

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_Orders()
    {
        // Arrange
        var orders = new List<OrderDto> { FakeOrderDto(1), FakeOrderDto(2) };
        SetupGetPagingList(FakePaginate(orders));

        // Act
        var result = await _handler.Handle(BuildQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_EmptyList_When_NoOrders()
    {
        // Arrange
        SetupGetPagingList(FakePaginate(new List<OrderDto>()));

        // Act
        var result = await _handler.Handle(BuildQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_SearchKeyword_Provided()
    {
        // Arrange
        var orders = new List<OrderDto> { FakeOrderDto(orderCode: "COD-NVA-20240101") };
        SetupGetPagingList(FakePaginate(orders));

        // Act
        var result = await _handler.Handle(BuildQuery(searchKeyword: "Nguyen"), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_IsSelf_Provided()
    {
        // Arrange
        var orders = new List<OrderDto> { FakeOrderDto() };
        SetupGetPagingList(FakePaginate(orders));

        // Act
        var result = await _handler.Handle(BuildQuery(isSelf: true), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_StatusFilter_Provided()
    {
        // Arrange
        var orders = new List<OrderDto> { FakeOrderDto(status: "Confirming") };
        SetupGetPagingList(FakePaginate(orders));

        // Act
        var result = await _handler.Handle(BuildQuery(status: OrderStatus.Confirming), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.All(o => o.Status == "Confirming").Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnCorrectPagination_When_PageAndSizeProvided()
    {
        // Arrange
        var orders = new List<OrderDto> { FakeOrderDto() };
        SetupGetPagingList(FakePaginate(orders, page: 2, size: 5, total: 11, totalPages: 3));

        // Act
        var result = await _handler.Handle(BuildQuery(page: 2, pageSize: 5), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Page.Should().Be(2);
        result.Data.Size.Should().Be(5);
        result.Data.Total.Should().Be(11);
        result.Data.TotalPages.Should().Be(3);
    }
}