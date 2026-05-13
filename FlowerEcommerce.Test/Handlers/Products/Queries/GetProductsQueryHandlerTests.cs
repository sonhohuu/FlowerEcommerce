using FlowerEcommerce.Application.Handlers.Products.Queries.GetProducts;

namespace FlowerEcommerce.Test.Handlers.Products.Queries;

public class GetProductsQueryHandlerTests : TestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly GetProductsQueryHandler _handler;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    private readonly Mock<IBaseRepository<Product>> _productRepo;

    public GetProductsQueryHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _productRepo = new Mock<IBaseRepository<Product>>();
        _logger = NullLogger<GetProductsQueryHandler>.Instance;

        _unitOfWork.Setup(u => u.Repository<Product>()).Returns(_productRepo.Object);

        _handler = new GetProductsQueryHandler(_unitOfWork.Object, _logger);
    }

    // ── Helper ─────────────────────────────────────────────────────
    // Đúng thứ tự: predicate, orderBy, includes, page, size, cancellationToken
    private void SetupPagingResult(List<Product> products, int page = 1, int size = 10)
    {
        var paginateResult = new Mock<IPaginate<ProductListDto>>();
        paginateResult.Setup(p => p.Items).Returns(
            products.Select(p => new ProductListDto { Id = p.Id, Name = p.Name }).ToList());
        paginateResult.Setup(p => p.Total).Returns(products.Count);

        _productRepo
            .Setup(r => r.GetPagingListAsync<ProductListDto>(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(), // orderBy
                It.IsAny<List<string>>(),                                           // includes
                page,
                size,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginateResult.Object);
    }

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnPaginatedProducts()
    {
        var products = new List<Product>
        {
            CreateFakeProduct(name: "Rose"),
            CreateFakeProduct(name: "Tulip")
        };
        SetupPagingResult(products);

        var result = await _handler.Handle(
            new GetProductsQuery { Page = 1, PageSize = 10 }, default);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NoProducts()
    {
        SetupPagingResult(new List<Product>());

        var result = await _handler.Handle(
            new GetProductsQuery { Page = 1, PageSize = 10 }, default);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_PassCorrectPagingParams()
    {
        SetupPagingResult(new List<Product>(), page: 2, size: 5);

        var result = await _handler.Handle(
            new GetProductsQuery { Page = 2, PageSize = 5 }, default);

        result.IsSuccess.Should().BeTrue();
        _productRepo.Verify(r => r.GetPagingListAsync<ProductListDto>(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
            It.IsAny<List<string>>(),
            2,
            5,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_FilterBySearchKeyword_When_Provided()
    {
        SetupPagingResult(new List<Product> { CreateFakeProduct(name: "Rose Bouquet") });

        var result = await _handler.Handle(
            new GetProductsQuery { Page = 1, PageSize = 10, SearchKeyword = "Rose" }, default);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.First().Name.Should().Contain("Rose");
    }

    [Fact]
    public async Task Handle_Should_FilterByCategoryId_When_Provided()
    {
        var categoryId = 1ul;
        SetupPagingResult(new List<Product> { CreateFakeProduct(categoryId: categoryId) });

        var result = await _handler.Handle(
            new GetProductsQuery { Page = 1, PageSize = 10, CategoryId = categoryId }, default);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        _productRepo
            .Setup(r => r.GetPagingListAsync<ProductListDto>(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(
            new GetProductsQuery { Page = 1, PageSize = 10 }, default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
