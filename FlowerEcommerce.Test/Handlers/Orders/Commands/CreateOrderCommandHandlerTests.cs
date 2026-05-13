using FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;
using FlowerEcommerce.Application.Handlers.Payment.CreatePaymentLink;
using MediatR;

namespace FlowerEcommerce.Test.Handlers.Orders.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly Mock<IMediator> _mediator;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly CreateOrderCommandHandler _handler;

    private readonly Mock<IBaseRepository<Product>> _productRepo;
    private readonly Mock<IBaseRepository<Order>> _orderRepo;

    public CreateOrderCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _currentUserService = new Mock<ICurrentUserService>();
        _mediator = new Mock<IMediator>();
        _logger = NullLogger<CreateOrderCommandHandler>.Instance;

        _productRepo = new Mock<IBaseRepository<Product>>();
        _orderRepo = new Mock<IBaseRepository<Order>>();

        _unitOfWork.Setup(u => u.Repository<Product>()).Returns(_productRepo.Object);
        _unitOfWork.Setup(u => u.Repository<Order>()).Returns(_orderRepo.Object);
        _currentUserService.Setup(s => s.UserId).Returns(1);

        _handler = new CreateOrderCommandHandler(
            _unitOfWork.Object,
            _logger,
            _currentUserService.Object,
            _mediator.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static CreateOrderCommand BuildCommand(
        PaymentMethod paymentMethod = PaymentMethod.COD,
        List<OrderItemDto>? items = null) =>
        new CreateOrderCommand
        {
            CustomerName = "Nguyen Van A",
            PaymentMethod = paymentMethod,
            Items = items ?? new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = 1, Quantity = 2, Price = 50_000 }
            }
        };

    private void SetupProductExists(bool exists) =>
        _productRepo.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(exists);

    private void SetupPaymentLink(bool isSuccess) =>
    _mediator.Setup(m => m.Send(
        It.IsAny<CreatePaymentLinkCommand>(),
        It.IsAny<CancellationToken>()))
    .ReturnsAsync(isSuccess
        ? TResult<CreatePaymentLinkResult>.Success(new CreatePaymentLinkResult(
            CheckoutUrl: "https://pay.example.com/checkout",
            QrCode: "qr-code-data",
            OrderCode: 123456789))
        : TResult<CreatePaymentLinkResult>.Failure("PayOS error"));

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        SetupProductExists(false);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_COD_Order()
    {
        // Arrange
        SetupProductExists(true);

        // Act
        var result = await _handler.Handle(BuildCommand(PaymentMethod.COD), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CheckoutUrl.Should().BeNull();
        result.Data.QrCode.Should().BeNull();
        _orderRepo.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_PaymentLink_When_BankTransfer()
    {
        // Arrange
        SetupProductExists(true);
        SetupPaymentLink(true);

        // Act
        var result = await _handler.Handle(BuildCommand(PaymentMethod.BankTransfer), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CheckoutUrl.Should().Be("https://pay.example.com/checkout");
        result.Data.QrCode.Should().Be("qr-code-data");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_Without_PaymentLink_When_BankTransfer_PayOSFails()
    {
        // Arrange
        SetupProductExists(true);
        SetupPaymentLink(false);

        // Act
        var result = await _handler.Handle(BuildCommand(PaymentMethod.BankTransfer), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CheckoutUrl.Should().BeNull();
        result.Data.QrCode.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_CalculateTotalAmount_Correctly()
    {
        // Arrange
        Order? savedOrder = null;
        SetupProductExists(true);
        _orderRepo.Setup(r => r.Add(It.IsAny<Order>()))
                  .Callback<Order>(o => savedOrder = o);

        var items = new List<OrderItemDto>
        {
            new OrderItemDto { ProductId = 1, Quantity = 2, Price = 50_000 },
            new OrderItemDto { ProductId = 2, Quantity = 1, Price = 30_000 }
        };

        // Act
        await _handler.Handle(BuildCommand(items: items), default);

        // Assert
        savedOrder.Should().NotBeNull();
        savedOrder!.TotalAmount.Should().Be(130_000);
    }

    [Fact]
    public async Task Handle_Should_SetUserId_From_CurrentUserService()
    {
        // Arrange
        Order? savedOrder = null;
        SetupProductExists(true);
        _currentUserService.Setup(s => s.UserId).Returns(42);
        _orderRepo.Setup(r => r.Add(It.IsAny<Order>()))
                  .Callback<Order>(o => savedOrder = o);

        // Act
        await _handler.Handle(BuildCommand(), default);

        // Assert
        savedOrder.Should().NotBeNull();
        savedOrder!.UserId.Should().Be(42);
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        // Arrange
        _productRepo.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()))
        .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}