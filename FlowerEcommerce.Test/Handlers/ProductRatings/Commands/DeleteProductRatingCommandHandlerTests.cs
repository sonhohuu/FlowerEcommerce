using FlowerEcommerce.Application.Handlers.ProductRatings.Commands.DeleteProductRating;

namespace FlowerEcommerce.Test.Handlers.ProductRatings.Commands;

public class DeleteProductRatingCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ILogger<DeleteProductRatingCommandHandler> _logger;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly DeleteProductRatingCommandHandler _handler;

    private readonly Mock<IBaseRepository<ProductRating>> _ratingRepo;

    private const ulong FakeUserId = 1;
    private const ulong FakeRatingId = 100;

    public DeleteProductRatingCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = NullLogger<DeleteProductRatingCommandHandler>.Instance;
        _currentUserService = new Mock<ICurrentUserService>();

        _ratingRepo = new Mock<IBaseRepository<ProductRating>>();

        _unitOfWork.Setup(u => u.Repository<ProductRating>()).Returns(_ratingRepo.Object);

        _currentUserService.Setup(s => s.UserId).Returns(FakeUserId);
        _currentUserService.Setup(s => s.IsAdmin).Returns(false);

        _handler = new DeleteProductRatingCommandHandler(
            _unitOfWork.Object,
            _logger,
            _currentUserService.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private DeleteProductRatingCommand BuildCommand(ulong id = FakeRatingId) =>
        new DeleteProductRatingCommand { Id = id };

    private void SetupRatingRepo(ProductRating? rating) =>
        _ratingRepo
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductRating, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<ProductRating>, IOrderedQueryable<ProductRating>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(rating);

    private ProductRating FakeRating(ulong? userId = null) => new ProductRating
    {
        Id = FakeRatingId,
        ProductId = 10,
        UserId = userId ?? FakeUserId,
        Score = 4,
        Comment = "Good"
    };

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RatingNotFound()
    {
        SetupRatingRepo(null);

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.NOT_FOUND);
    }

    [Fact]
    public async Task Handle_Should_ReturnForbidden_When_UserIsNotOwner()
    {
        // Rating thuộc về user khác
        SetupRatingRepo(FakeRating(userId: 999));
        _currentUserService.Setup(s => s.IsAdmin).Returns(false);

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.FORBIDDEN);
        _ratingRepo.Verify(r => r.Remove(It.IsAny<ProductRating>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_UserIsOwner()
    {
        SetupRatingRepo(FakeRating(userId: FakeUserId));

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeTrue();
        _ratingRepo.Verify(r => r.Remove(It.IsAny<ProductRating>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_UserIsAdmin()
    {
        // Admin xóa rating của người khác
        SetupRatingRepo(FakeRating(userId: 999));
        _currentUserService.Setup(s => s.IsAdmin).Returns(true);

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeTrue();
        _ratingRepo.Verify(r => r.Remove(It.IsAny<ProductRating>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        _ratingRepo
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductRating, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<ProductRating>, IOrderedQueryable<ProductRating>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
