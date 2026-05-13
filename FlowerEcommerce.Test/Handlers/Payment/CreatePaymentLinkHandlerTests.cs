using FlowerEcommerce.Application.Common.Configs;
using FlowerEcommerce.Application.Handlers.Payment.CreatePaymentLink;
using Microsoft.Extensions.Options;

namespace FlowerEcommerce.Test.Handlers.Payment;

public class CreatePaymentLinkHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IPayOSService> _payOS;
    private readonly Mock<IOptions<PayOSOptions>> _options;
    private readonly CreatePaymentLinkHandler _handler;
    private readonly Mock<IBaseRepository<Order>> _orderRepo;

    public CreatePaymentLinkHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _payOS = new Mock<IPayOSService>();
        _options = new Mock<IOptions<PayOSOptions>>();

        _options.Setup(o => o.Value).Returns(new PayOSOptions
        {
            ReturnUrl = "https://example.com/return",
            CancelUrl = "https://example.com/cancel"
        });

        _orderRepo = new Mock<IBaseRepository<Order>>();
        _unitOfWork.Setup(u => u.Repository<Order>()).Returns(_orderRepo.Object);

        _handler = new CreatePaymentLinkHandler(
            _unitOfWork.Object,
            _payOS.Object,
            _options.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static CreatePaymentLinkCommand BuildCommand(ulong orderId = 1) =>
        new CreatePaymentLinkCommand { OrderId = orderId };

    private static Order FakeOrder(ulong id = 1) =>
        new Order
        {
            Id = id,
            OrderCode = "BTF-NVA-20240101",
            CustomerName = "Nguyen Van A",
            PhoneNumber = "0901234567",
            TotalAmount = 100_000,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    Quantity = 2,
                    Price    = 50_000,
                    Product  = new Product { Name = "Hoa Hồng" }
                }
            }
        };

    private static CreatePaymentResult FakePaymentResult() =>
        new CreatePaymentResult(
            CheckoutUrl: "https://pay.example.com/checkout",
            QrCode: "qr-code-data",
            PaymentLinkId: "payment-link-id",
            OrderCode: 1
        );

    private void SetupFirstOrDefault(Order? returns) =>
        _orderRepo.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Order, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    private void SetupCreatePaymentLink(CreatePaymentResult? returns = null) =>
        _payOS.Setup(p => p.CreatePaymentLinkAsync(It.IsAny<CreatePaymentRequest>()))
              .ReturnsAsync(returns ?? FakePaymentResult());

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
        _payOS.Verify(p => p.CreatePaymentLinkAsync(It.IsAny<CreatePaymentRequest>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_PaymentLink_When_OrderFound()
    {
        // Arrange
        SetupFirstOrDefault(FakeOrder());
        SetupCreatePaymentLink();

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.CheckoutUrl.Should().Be("https://pay.example.com/checkout");
        result.Data.QrCode.Should().Be("qr-code-data");
    }

    [Fact]
    public async Task Handle_Should_PassCorrectAmount_To_PayOS()
    {
        // Arrange
        CreatePaymentRequest? capturedRequest = null;
        SetupFirstOrDefault(FakeOrder());
        _payOS.Setup(p => p.CreatePaymentLinkAsync(It.IsAny<CreatePaymentRequest>()))
              .Callback<CreatePaymentRequest>(r => capturedRequest = r)
              .ReturnsAsync(FakePaymentResult());

        // Act
        await _handler.Handle(BuildCommand(), default);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.Amount.Should().Be(100_000);
        capturedRequest.OrderCode.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_PassCorrectItems_To_PayOS()
    {
        // Arrange
        CreatePaymentRequest? capturedRequest = null;
        SetupFirstOrDefault(FakeOrder());
        _payOS.Setup(p => p.CreatePaymentLinkAsync(It.IsAny<CreatePaymentRequest>()))
              .Callback<CreatePaymentRequest>(r => capturedRequest = r)
              .ReturnsAsync(FakePaymentResult());

        // Act
        await _handler.Handle(BuildCommand(), default);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.Items.Should().HaveCount(1);
        capturedRequest.Items.First().Name.Should().Be("Hoa Hồng");
        capturedRequest.Items.First().Quantity.Should().Be(2);
        capturedRequest.Items.First().Price.Should().Be(50_000);
    }

    [Fact]
    public async Task Handle_Should_PassReturnUrlAndCancelUrl_To_PayOS()
    {
        // Arrange
        CreatePaymentRequest? capturedRequest = null;
        SetupFirstOrDefault(FakeOrder());
        _payOS.Setup(p => p.CreatePaymentLinkAsync(It.IsAny<CreatePaymentRequest>()))
              .Callback<CreatePaymentRequest>(r => capturedRequest = r)
              .ReturnsAsync(FakePaymentResult());

        // Act
        await _handler.Handle(BuildCommand(), default);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.ReturnUrl.Should().Be("https://example.com/return");
        capturedRequest.CancelUrl.Should().Be("https://example.com/cancel");
    }
}