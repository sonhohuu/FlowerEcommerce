using FlowerEcommerce.Application.Handlers.Payment.HandlePayOSWebhook;
using Microsoft.Extensions.Logging;

namespace FlowerEcommerce.Test.Handlers.Payment;

public class HandlePayOSWebhookHandlerTests : TestBase
{
    private readonly Mock<IPayOSService> _payOS;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<HandlePayOSWebhookHandler>> _logger;
    private readonly HandlePayOSWebhookHandler _handler;

    private readonly Mock<IBaseRepository<Order>> _orderRepo;

    public HandlePayOSWebhookHandlerTests()
    {
        _payOS = new Mock<IPayOSService>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<HandlePayOSWebhookHandler>>();

        _orderRepo = new Mock<IBaseRepository<Order>>();
        _unitOfWork.Setup(u => u.Repository<Order>()).Returns(_orderRepo.Object);

        _handler = new HandlePayOSWebhookHandler(
            _payOS.Object,
            _unitOfWork.Object,
            _logger.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static WebhookDataPayload FakePayload(long orderCode = 1, int amount = 100_000) =>
        new WebhookDataPayload(
            OrderCode: orderCode,
            Amount: amount,
            Description: "Thanh toan don hang",
            AccountNumber: "123456789",
            Reference: "REF-001",
            TransactionDateTime: "2024-01-01T00:00:00",
            Currency: "VND",
            PaymentLinkId: "payment-link-id",
            Code: "00",
            Desc: "success"
        );

    private static HandlePayOSWebhookCommand BuildCommand(
        string code = "00",
        long orderCode = 1,
        int amount = 100_000) =>
        new HandlePayOSWebhookCommand
        {
            Data = new WebhookData(
                Code: code,
                Desc: "success",
                Success: true,
                Data: FakePayload(orderCode, amount),
                Signature: "fake-signature"
            )
        };

    private static HandlePayOSWebhookCommand BuildCommandNullData(string code = "01") =>
        new HandlePayOSWebhookCommand
        {
            Data = new WebhookData(
                Code: code,
                Desc: "failed",
                Success: false,
                Data: null,
                Signature: "fake-signature"
            )
        };

    private static Order FakeOrder(ulong id = 1, OrderStatus status = OrderStatus.Confirming) =>
        new Order
        {
            Id = id,
            Status = status
        };

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
    public async Task Handle_Should_ReturnSuccess_When_CodeIsNonSuccess()
    {
        // Arrange
        var command = BuildCommandNullData(code: "01");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _orderRepo.Verify(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Order, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_DataIsNull()
    {
        // Arrange
        var command = BuildCommandNullData(code: "00");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _orderRepo.Verify(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Order, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrderNotFound()
    {
        // Arrange
        SetupFirstOrDefault(null);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_OrderAlreadyProcessing()
    {
        // Arrange
        SetupFirstOrDefault(FakeOrder(status: OrderStatus.Processing));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateOrderStatus_To_Processing_When_PaymentSuccess()
    {
        // Arrange
        var order = FakeOrder(status: OrderStatus.Confirming);
        SetupFirstOrDefault(order);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Processing);
        _orderRepo.Verify(r => r.Update(order), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_NotSave_When_OrderAlreadyProcessing()
    {
        // Arrange
        SetupFirstOrDefault(FakeOrder(status: OrderStatus.Processing));

        // Act
        await _handler.Handle(BuildCommand(), default);

        // Assert
        _orderRepo.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}