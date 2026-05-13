using FlowerEcommerce.Application.Handlers.Categories.Commands.UpdateCategory;

namespace FlowerEcommerce.Test.Handlers.Categories.Commands;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;
    private readonly UpdateCategoryCommandHandler _handler;
    private readonly Mock<IBaseRepository<Category>> _categoryRepo;

    public UpdateCategoryCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = NullLogger<UpdateCategoryCommandHandler>.Instance;
        _categoryRepo = new Mock<IBaseRepository<Category>>();

        _unitOfWork.Setup(u => u.Repository<Category>()).Returns(_categoryRepo.Object);

        _handler = new UpdateCategoryCommandHandler(_unitOfWork.Object, _logger);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static UpdateCategoryCommand BuildCommand(ulong id = 1, string name = "Hoa Cúc") =>
        new UpdateCategoryCommand { Id = id, Name = name };

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

    private void SetupAnyAsync(bool returns) =>
        _categoryRepo.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Category, bool>>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryNameAlreadyExists()
    {
        // Arrange
        SetupAnyAsync(true);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ALREADY_EXISTS);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryNotFound()
    {
        // Arrange
        SetupAnyAsync(false);
        SetupFirstOrDefault(null);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.NOT_FOUND);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CategoryUpdated()
    {
        // Arrange
        SetupAnyAsync(false);
        SetupFirstOrDefault(FakeCategory());

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _categoryRepo.Verify(r => r.Update(It.IsAny<Category>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_TrimNameAndGenerateSlug_When_CategoryUpdated()
    {
        // Arrange
        Category? updatedCategory = null;

        SetupAnyAsync(false);
        SetupFirstOrDefault(FakeCategory());
        _categoryRepo
            .Setup(r => r.Update(It.IsAny<Category>()))
            .Callback<Category>(c => updatedCategory = c);

        // Act
        await _handler.Handle(BuildCommand(name: "  Hoa Cúc  "), default);

        // Assert
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Name.Should().Be("Hoa Cúc");
        updatedCategory.Slug.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        // Arrange
        _categoryRepo.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Category, bool>>>(),
            It.IsAny<CancellationToken>()))
        .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
