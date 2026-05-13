using FlowerEcommerce.Application.Handlers.Orders.Commands.DeleteOrder;

namespace FlowerEcommerce.Test.Handlers.Orders.Commands;

public class DeleteOrderCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ILogger<DeleteOrderCommandHandler> _logger;
    private readonly DeleteOrderCommandHandler _handler;
    private readonly Mock<IBaseRepository<Order>> _orderRepo;

    public DeleteOrderCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = NullLogger<DeleteOrderCommandHandler>.Instance;
        _orderRepo = new Mock<IBaseRepository<Order>>();

        _unitOfWork.Setup(u => u.Repository<Order>()).Returns(_orderRepo.Object);

        _handler = new DeleteOrderCommandHandler(_unitOfWork.Object, _logger);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static DeleteOrderCommand BuildCommand(ulong id = 1) =>
        new DeleteOrderCommand { Id = id };

    private static Order FakeOrder(
        ulong id = 1,
        OrderStatus status = OrderStatus.Confirming) =>
        new Order { Id = id, Status = status };

    private void SetupFirstOrDefault(Order? returns) =>
        _orderRepo.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Order, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrderNotFound()
    {
        // Arrange
        SetupFirstOrDefault(null);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Theory]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.Success)]
    [InlineData(OrderStatus.Failed)]
    [InlineData(OrderStatus.OnDelivering)]
    public async Task Handle_Should_ReturnFailure_When_OrderIsNotConfirming(OrderStatus status)
    {
        // Arrange
        SetupFirstOrDefault(FakeOrder(status: status));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_SetStatusToFailed_When_Confirming()
    {
        // Arrange
        Order? updatedOrder = null;
        SetupFirstOrDefault(FakeOrder(status: OrderStatus.Confirming));
        _orderRepo.Setup(r => r.Update(It.IsAny<Order>()))
                  .Callback<Order>(o => updatedOrder = o);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        updatedOrder.Should().NotBeNull();
        updatedOrder!.Status.Should().Be(OrderStatus.Failed);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        // Arrange
        _orderRepo.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Order, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
        .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
