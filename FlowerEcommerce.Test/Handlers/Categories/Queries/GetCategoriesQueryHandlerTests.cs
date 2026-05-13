using FlowerEcommerce.Application.Handlers.Categories.Queries.GetCategories;

namespace FlowerEcommerce.Test.Handlers.Categories.Queries;

public class GetCategoriesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly GetCategoriesQueryHandler _handler;
    private readonly Mock<IBaseRepository<Category>> _categoryRepo;

    public GetCategoriesQueryHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _categoryRepo = new Mock<IBaseRepository<Category>>();

        _unitOfWork.Setup(u => u.Repository<Category>()).Returns(_categoryRepo.Object);

        _handler = new GetCategoriesQueryHandler(_unitOfWork.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static GetCategoriesQuery BuildQuery() => new GetCategoriesQuery();

    private void SetupGetAllAsync(List<Category> returns) =>
        _categoryRepo.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Category, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_AllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Hoa Hồng",  Slug = "hoa-hong"  },
            new Category { Id = 2, Name = "Hoa Cúc",   Slug = "hoa-cuc"   },
            new Category { Id = 3, Name = "Hoa Tulip",  Slug = "hoa-tulip" },
        };
        SetupGetAllAsync(categories);

        // Act
        var result = await _handler.Handle(BuildQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_EmptyList_When_NoCategoriesExist()
    {
        // Arrange
        SetupGetAllAsync(new List<Category>());

        // Act
        var result = await _handler.Handle(BuildQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_MapCorrectly_When_CategoriesReturned()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Hoa Hồng", Slug = "hoa-hong" }
        };
        SetupGetAllAsync(categories);

        // Act
        var result = await _handler.Handle(BuildQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.First().Id.Should().Be(1);
        result.Data.First().Name.Should().Be("Hoa Hồng");
        result.Data.First().Slug.Should().Be("hoa-hong");
    }
}
