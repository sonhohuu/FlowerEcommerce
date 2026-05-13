using FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;
using FlowerEcommerce.Application.Handlers.Orders.Commands.UpdateOrder;

namespace FlowerEcommerce.Test.Handlers.Orders.Commands;

public class UpdateOrderCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;
    private readonly UpdateOrderCommandHandler _handler;

    private readonly Mock<IBaseRepository<Order>> _orderRepo;
    private readonly Mock<IBaseRepository<Product>> _productRepo;
    private readonly Mock<IBaseRepository<OrderItem>> _orderItemRepo;

    public UpdateOrderCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = NullLogger<UpdateOrderCommandHandler>.Instance;

        _orderRepo = new Mock<IBaseRepository<Order>>();
        _productRepo = new Mock<IBaseRepository<Product>>();
        _orderItemRepo = new Mock<IBaseRepository<OrderItem>>();

        _unitOfWork.Setup(u => u.Repository<Order>()).Returns(_orderRepo.Object);
        _unitOfWork.Setup(u => u.Repository<Product>()).Returns(_productRepo.Object);
        _unitOfWork.Setup(u => u.Repository<OrderItem>()).Returns(_orderItemRepo.Object);

        _handler = new UpdateOrderCommandHandler(_unitOfWork.Object, _logger);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static UpdateOrderCommand BuildCommand(
        ulong id = 1,
        OrderStatus status = OrderStatus.Processing,
        List<OrderItemDto>? items = null) =>
        new UpdateOrderCommand
        {
            Id = id,
            Status = status,
            CustomerName = "Nguyen Van A",
            Items = items
        };

    private static Order FakeOrder(
        ulong id = 1,
        OrderStatus status = OrderStatus.Confirming) =>
        new Order
        {
            Id = id,
            Status = status,
            CustomerName = "Nguyen Van A",
            PaymentMethod = PaymentMethod.COD,
            Items = new List<OrderItem>()
        };

    private void SetupFirstOrDefault(Order? returns) =>
        _orderRepo.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Order, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    private void SetupProductExists(bool exists) =>
        _productRepo.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(exists);

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

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrderIsSuccess()
    {
        // Arrange
        SetupFirstOrDefault(FakeOrder(status: OrderStatus.Success));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrderIsFailed()
    {
        // Arrange
        SetupFirstOrDefault(FakeOrder(status: OrderStatus.Failed));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_StatusTransitionIsBackward()
    {
        // Arrange
        SetupFirstOrDefault(FakeOrder(status: OrderStatus.Processing));

        // Act — cố gắng lùi về Confirming
        var result = await _handler.Handle(BuildCommand(status: OrderStatus.Confirming), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_StatusUpdated()
    {
        // Arrange
        SetupFirstOrDefault(FakeOrder(status: OrderStatus.Confirming));

        // Act
        var result = await _handler.Handle(
            BuildCommand(status: OrderStatus.Processing, items: new List<OrderItemDto>()),
            default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _orderRepo.Verify(r => r.Update(It.IsAny<Order>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Confirming_And_ProductNotFound()
    {
        // Arrange
        var items = new List<OrderItemDto>
        {
            new OrderItemDto { ProductId = 99, Quantity = 1, Price = 50_000 }
        };
        SetupFirstOrDefault(FakeOrder(status: OrderStatus.Confirming));
        SetupProductExists(false);

        // Act
        var result = await _handler.Handle(BuildCommand(items: items), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReplaceItems_And_RecalculateTotal_When_Confirming()
    {
        // Arrange
        Order? updatedOrder = null;
        var newItems = new List<OrderItemDto>
        {
            new OrderItemDto { ProductId = 1, Quantity = 3, Price = 40_000 },
            new OrderItemDto { ProductId = 2, Quantity = 1, Price = 20_000 }
        };

        SetupFirstOrDefault(FakeOrder(status: OrderStatus.Confirming));
        SetupProductExists(true);
        _orderRepo.Setup(r => r.Update(It.IsAny<Order>()))
                  .Callback<Order>(o => updatedOrder = o);

        // Act
        await _handler.Handle(BuildCommand(items: newItems), default);

        // Assert
        updatedOrder.Should().NotBeNull();
        updatedOrder!.TotalAmount.Should().Be(140_000);
        _orderItemRepo.Verify(r => r.RemoveRange(It.IsAny<IEnumerable<OrderItem>>()), Times.Once);
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
