using FlowerEcommerce.Application.Handlers.ProductRatings.Commands.UpsertProductRating;

namespace FlowerEcommerce.Test.Handlers.ProductRatings.Commands;

public class UpsertProductRatingCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ILogger<UpsertProductRatingCommandHandler> _logger;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly UpsertProductRatingCommandHandler _handler;

    private readonly Mock<IBaseRepository<Product>> _productRepo;
    private readonly Mock<IBaseRepository<ProductRating>> _ratingRepo;

    private const ulong FakeUserId = 1;
    private const ulong FakeProductId = 10;

    public UpsertProductRatingCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = NullLogger<UpsertProductRatingCommandHandler>.Instance;
        _currentUserService = new Mock<ICurrentUserService>();

        _productRepo = new Mock<IBaseRepository<Product>>();
        _ratingRepo = new Mock<IBaseRepository<ProductRating>>();

        _unitOfWork.Setup(u => u.Repository<Product>()).Returns(_productRepo.Object);
        _unitOfWork.Setup(u => u.Repository<ProductRating>()).Returns(_ratingRepo.Object);

        _currentUserService.Setup(s => s.UserId).Returns(FakeUserId);

        _handler = new UpsertProductRatingCommandHandler(
            _unitOfWork.Object,
            _logger,
            _currentUserService.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private UpsertProductRatingCommand BuildCommand(
        ulong productId = FakeProductId,
        int score = 5,
        string? comment = "Great!") => new UpsertProductRatingCommand
        {
            ProductId = productId,
            Score = score,
            Comment = comment
        };

    private void SetupProductExists(bool exists) =>
        _productRepo
            .Setup(r => r.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists);

    private void SetupExistingRating(ProductRating? rating) =>
        _ratingRepo
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductRating, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<ProductRating>, IOrderedQueryable<ProductRating>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(rating);

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductNotFound()
    {
        SetupProductExists(false);

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.NOT_FOUND);
    }

    [Fact]
    public async Task Handle_Should_CreateRating_When_NoExistingRating()
    {
        SetupProductExists(true);
        SetupExistingRating(null);

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeTrue();
        _ratingRepo.Verify(r => r.Add(It.IsAny<ProductRating>()), Times.Once);
        _ratingRepo.Verify(r => r.Update(It.IsAny<ProductRating>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_SetCorrectUserId_When_CreatingRating()
    {
        ProductRating? savedRating = null;
        SetupProductExists(true);
        SetupExistingRating(null);
        _ratingRepo
            .Setup(r => r.Add(It.IsAny<ProductRating>()))
            .Callback<ProductRating>(r => savedRating = r);

        await _handler.Handle(BuildCommand(), default);

        savedRating.Should().NotBeNull();
        savedRating!.UserId.Should().Be(FakeUserId);
    }

    [Fact]
    public async Task Handle_Should_UpdateRating_When_ExistingRatingFound()
    {
        var existingRating = new ProductRating
        {
            Id = 1,
            ProductId = FakeProductId,
            UserId = FakeUserId,
            Score = 3,
            Comment = "Old comment"
        };
        SetupProductExists(true);
        SetupExistingRating(existingRating);

        var result = await _handler.Handle(BuildCommand(score: 5, comment: "New comment"), default);

        result.IsSuccess.Should().BeTrue();
        _ratingRepo.Verify(r => r.Update(existingRating), Times.Once);
        _ratingRepo.Verify(r => r.Add(It.IsAny<ProductRating>()), Times.Never);
        existingRating.Score.Should().Be(5);
        existingRating.Comment.Should().Be("New comment");
    }

    [Fact]
    public async Task Handle_Should_KeepOldComment_When_NewCommentIsEmpty()
    {
        var existingRating = new ProductRating
        {
            Id = 1,
            ProductId = FakeProductId,
            UserId = FakeUserId,
            Score = 3,
            Comment = "Old comment"
        };
        SetupProductExists(true);
        SetupExistingRating(existingRating);

        await _handler.Handle(BuildCommand(score: 4, comment: ""), default);

        existingRating.Comment.Should().Be("Old comment");
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        _productRepo
            .Setup(r => r.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
