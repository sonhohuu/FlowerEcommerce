using FlowerEcommerce.Application.Handlers.Products.Commands.DeleteProduct;

namespace FlowerEcommerce.Test.Handlers.Products.Commands;

public class DeleteProductCommandHandlerTests : TestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ILogger<DeleteProductCommandHandler> _logger;
    private readonly DeleteProductCommandHandler _handler;

    private readonly Mock<IBaseRepository<Product>> _productRepo;

    public DeleteProductCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = NullLogger<DeleteProductCommandHandler>.Instance;
        _productRepo = new Mock<IBaseRepository<Product>>();

        _unitOfWork.Setup(u => u.Repository<Product>()).Returns(_productRepo.Object);

        _handler = new DeleteProductCommandHandler(_unitOfWork.Object, _logger);
    }

    // ── Helper ─────────────────────────────────────────────────────
    // Handler chỉ truyền predicate, các param còn lại là default
    // nên mock đúng full signature với It.IsAny cho tất cả
    private void SetupProductRepo(Product? returns) =>
        _productRepo
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns);

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductNotFound()
    {
        SetupProductRepo(null);

        var result = await _handler.Handle(
            new DeleteProductCommand { Id = 1 }, default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.NOT_FOUND);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_RemoveProduct_When_Found()
    {
        var product = CreateFakeProduct();
        SetupProductRepo(product);

        var result = await _handler.Handle(
            new DeleteProductCommand { Id = product.Id }, default);

        result.IsSuccess.Should().BeTrue();
        _productRepo.Verify(r => r.Remove(product), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        _productRepo
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(
            new DeleteProductCommand { Id = 1 }, default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
