using FlowerEcommerce.Application.Handlers.Products.Queries.GetProductById;

namespace FlowerEcommerce.Test.Handlers.Products.Queries;

public class GetProductByIdQueryHandlerTests : TestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly GetProductByIdQueryHandler _handler;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    private readonly Mock<IBaseRepository<Product>> _productRepo;

    public GetProductByIdQueryHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _productRepo = new Mock<IBaseRepository<Product>>();
        _logger = NullLogger<GetProductByIdQueryHandler>.Instance;

        _unitOfWork.Setup(u => u.Repository<Product>()).Returns(_productRepo.Object);

        _handler = new GetProductByIdQueryHandler(_unitOfWork.Object, _logger);
    }

    // ── Helper ─────────────────────────────────────────────────────
    // Handler gọi: predicate, includes(List<string>), asNoTracking: true
    // orderBy không truyền nên là default(null)
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
            new GetProductByIdQuery { Id = 1 }, default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.NOT_FOUND);
    }

    [Fact]
    public async Task Handle_Should_ReturnProductDto_When_Found()
    {
        var product = CreateFakeProduct();
        SetupProductRepo(product);

        var result = await _handler.Handle(
            new GetProductByIdQuery { Id = product.Id }, default);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(product.Id);
        result.Data.Name.Should().Be(product.Name);
    }

    [Fact]
    public async Task Handle_Should_IncludeCorrectRelations()
    {
        var product = CreateFakeProduct();
        List<string>? capturedIncludes = null;  // ← List<string>, không phải string[]

        _productRepo
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<
                Expression<Func<Product, bool>>,
                List<string>,                   // ← List<string>
                Func<IQueryable<Product>, IOrderedQueryable<Product>>,
                bool,
                CancellationToken>(
                (_, includes, _, _, _) => capturedIncludes = includes)
            .ReturnsAsync(product);

        await _handler.Handle(new GetProductByIdQuery { Id = product.Id }, default);

        capturedIncludes.Should().Contain(nameof(Product.ProductDetail));
        capturedIncludes.Should().Contain(nameof(Product.Category));
        capturedIncludes.Should().Contain(nameof(Product.FileAttachments));
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
            .ThrowsAsync(new Exception("DB timeout"));

        var result = await _handler.Handle(
            new GetProductByIdQuery { Id = 1 }, default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
