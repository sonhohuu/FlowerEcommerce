using FlowerEcommerce.Application.Handlers.Categories.Commands.CreateCategory;

namespace FlowerEcommerce.Test.Handlers.Categories.Commands;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;
    private readonly CreateCategoryCommandHandler _handler;

    // ── Shared repo mocks ──────────────────────────────────────────
    private readonly Mock<IBaseRepository<Category>> _categoryRepo;

    public CreateCategoryCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = NullLogger<CreateCategoryCommandHandler>.Instance;

        _categoryRepo = new Mock<IBaseRepository<Category>>();

        _unitOfWork.Setup(u => u.Repository<Category>()).Returns(_categoryRepo.Object);

        _handler = new CreateCategoryCommandHandler(
            _unitOfWork.Object,
            _logger);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private CreateCategoryCommand BuildCommand(string name = "Hoa Hồng") =>
        new CreateCategoryCommand
        {
            Name = name
        };

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryNameAlreadyExists()
    {
        // Arrange
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ALREADY_EXISTS);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CategoryCreated()
    {
        // Arrange
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _categoryRepo.Verify(r => r.Add(It.IsAny<Category>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_TrimName_When_NameHasWhitespace()
    {
        // Arrange
        Category? savedCategory = null;
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ReturnsAsync(false);
        _categoryRepo
            .Setup(r => r.Add(It.IsAny<Category>()))
            .Callback<Category>(c => savedCategory = c);

        // Act
        await _handler.Handle(BuildCommand(name: "  Hoa Hồng  "), default);

        // Assert
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be("Hoa Hồng");
    }

    [Fact]
    public async Task Handle_Should_GenerateSlug_When_CategoryCreated()
    {
        // Arrange
        Category? savedCategory = null;
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ReturnsAsync(false);
        _categoryRepo
            .Setup(r => r.Add(It.IsAny<Category>()))
            .Callback<Category>(c => savedCategory = c);

        // Act
        await _handler.Handle(BuildCommand(name: "Hoa Hồng"), default);

        // Assert
        savedCategory.Should().NotBeNull();
        savedCategory!.Slug.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        // Arrange
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
