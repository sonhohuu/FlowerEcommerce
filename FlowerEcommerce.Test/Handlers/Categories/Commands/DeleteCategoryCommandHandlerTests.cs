using FlowerEcommerce.Application.Handlers.Categories.Commands.DeleteCategory;

namespace FlowerEcommerce.Test.Handlers.Categories.Commands;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;
    private readonly DeleteCategoryCommandHandler _handler;
    private readonly Mock<IBaseRepository<Category>> _categoryRepo;

    public DeleteCategoryCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = NullLogger<DeleteCategoryCommandHandler>.Instance;
        _categoryRepo = new Mock<IBaseRepository<Category>>();

        _unitOfWork.Setup(u => u.Repository<Category>()).Returns(_categoryRepo.Object);

        _handler = new DeleteCategoryCommandHandler(_unitOfWork.Object, _logger);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static DeleteCategoryCommand BuildCommand(ulong id = 1) =>
        new DeleteCategoryCommand { Id = id };

    private static Category FakeCategory(ulong id = 1) =>
        new Category { Id = id, Name = "Hoa Hồng", Slug = "hoa-hong" };

    private void SetupFirstOrDefault(Category? returns) =>
        _categoryRepo.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Category, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryNotFound()
    {
        // Arrange
        SetupFirstOrDefault(null);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.NOT_FOUND);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CategoryDeleted()
    {
        // Arrange
        SetupFirstOrDefault(FakeCategory());

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _categoryRepo.Verify(r => r.Remove(It.IsAny<Category>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_RemoveCorrectCategory_When_CategoryExists()
    {
        // Arrange
        var category = FakeCategory(id: 42);
        Category? removedCategory = null;

        SetupFirstOrDefault(category);
        _categoryRepo
            .Setup(r => r.Remove(It.IsAny<Category>()))
            .Callback<Category>(c => removedCategory = c);

        // Act
        await _handler.Handle(BuildCommand(id: 42), default);

        // Assert
        removedCategory.Should().NotBeNull();
        removedCategory!.Id.Should().Be(42);
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        // Arrange
        _categoryRepo.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Category, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
        .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
