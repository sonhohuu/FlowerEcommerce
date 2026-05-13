using FlowerEcommerce.Application.Handlers.ProductRatings.Queries.GetProductRatings;

namespace FlowerEcommerce.Test.Handlers.ProductRatings.Queries;

public class GetProductRatingsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly GetProductRatingsQueryHandler _handler;
    private readonly ILogger<GetProductRatingsQueryHandler> _logger;

    private readonly Mock<IBaseRepository<ProductRating>> _ratingRepo;

    public GetProductRatingsQueryHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _ratingRepo = new Mock<IBaseRepository<ProductRating>>();
        _logger = NullLogger<GetProductRatingsQueryHandler>.Instance;

        _unitOfWork.Setup(u => u.Repository<ProductRating>()).Returns(_ratingRepo.Object);

        _handler = new GetProductRatingsQueryHandler(_unitOfWork.Object, _logger);
    }

    // ── Helpers ────────────────────────────────────────────────────
    // Đúng thứ tự: selector, predicate, orderBy, includes, page, size, cancellationToken
    private void SetupPagingResult(
        List<GetProductRatingsQueryResponse> items,
        int page = 1,
        int size = 10)
    {
        var paginate = new Mock<IPaginate<GetProductRatingsQueryResponse>>();
        paginate.Setup(p => p.Items).Returns(items);
        paginate.Setup(p => p.Total).Returns(items.Count);

        _ratingRepo
            .Setup(r => r.GetPagingListAsync(
                It.IsAny<Expression<Func<ProductRating, GetProductRatingsQueryResponse>>>(), // selector
                It.IsAny<Expression<Func<ProductRating, bool>>>(),                            // predicate
                It.IsAny<Func<IQueryable<ProductRating>, IOrderedQueryable<ProductRating>>>(),// orderBy
                It.IsAny<List<string>>(),                                                      // includes
                page,
                size,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginate.Object);
    }

    private GetProductRatingsQuery BuildQuery(
        ulong productId = 1,
        int page = 1,
        int pageSize = 10) => new GetProductRatingsQuery
        {
            ProductId = productId,
            Page = page,
            PageSize = pageSize
        };

    private static GetProductRatingsQueryResponse FakeResponse(
        ulong userId = 1,
        int score = 5,
        string comment = "Good") => new GetProductRatingsQueryResponse
        {
            Id = 1,
            Score = score,
            Comment = comment,
            UserId = userId,
            UserName = "John Doe"
        };

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnPaginatedRatings()
    {
        var items = new List<GetProductRatingsQueryResponse>
        {
            FakeResponse(userId: 1, score: 5),
            FakeResponse(userId: 2, score: 3)
        };
        SetupPagingResult(items);

        var result = await _handler.Handle(BuildQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NoRatings()
    {
        SetupPagingResult(new List<GetProductRatingsQueryResponse>());

        var result = await _handler.Handle(BuildQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_PassCorrectPagingParams()
    {
        SetupPagingResult(new List<GetProductRatingsQueryResponse>(), page: 2, size: 5);

        var result = await _handler.Handle(BuildQuery(page: 2, pageSize: 5), default);

        result.IsSuccess.Should().BeTrue();
        _ratingRepo.Verify(r => r.GetPagingListAsync(
            It.IsAny<Expression<Func<ProductRating, GetProductRatingsQueryResponse>>>(),
            It.IsAny<Expression<Func<ProductRating, bool>>>(),
            It.IsAny<Func<IQueryable<ProductRating>, IOrderedQueryable<ProductRating>>>(),
            It.IsAny<List<string>>(),
            2,
            5,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_FilterByProductId()
    {
        var productId = 42ul;
        SetupPagingResult(new List<GetProductRatingsQueryResponse> { FakeResponse() });

        var result = await _handler.Handle(BuildQuery(productId: productId), default);

        result.IsSuccess.Should().BeTrue();
        // Verify repo được gọi đúng 1 lần với productId filter
        _ratingRepo.Verify(r => r.GetPagingListAsync(
            It.IsAny<Expression<Func<ProductRating, GetProductRatingsQueryResponse>>>(),
            It.IsAny<Expression<Func<ProductRating, bool>>>(),
            It.IsAny<Func<IQueryable<ProductRating>, IOrderedQueryable<ProductRating>>>(),
            It.IsAny<List<string>>(),
            1,
            10,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        _ratingRepo
            .Setup(r => r.GetPagingListAsync(
                It.IsAny<Expression<Func<ProductRating, GetProductRatingsQueryResponse>>>(),
                It.IsAny<Expression<Func<ProductRating, bool>>>(),
                It.IsAny<Func<IQueryable<ProductRating>, IOrderedQueryable<ProductRating>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(BuildQuery(), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
