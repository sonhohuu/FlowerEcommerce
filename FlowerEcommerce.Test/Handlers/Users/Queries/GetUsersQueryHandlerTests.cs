using FlowerEcommerce.Application.Handlers.Users.Queries;


namespace FlowerEcommerce.Test.Handlers.Users.Queries;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _handler = new GetUsersQueryHandler(_userRepository.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private void SetupPagingResult(
        List<UserDto> items,
        int page = 1,
        int size = 10)
    {
        var paginate = new Mock<IPaginate<UserDto>>();
        paginate.Setup(p => p.Items).Returns(items);
        paginate.Setup(p => p.Total).Returns(items.Count);

        _userRepository
            .Setup(r => r.GetPagingListAsync(
                It.IsAny<Expression<Func<ApplicationUser, UserDto>>>(),  // selector
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),     // predicate
                It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(), // orderBy
                It.IsAny<List<string>>(),                                // includes
                page,
                size,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginate.Object);
    }

    private GetUsersQuery BuildQuery(
        int page = 1,
        int pageSize = 10,
        string? searchKeyword = null) => new GetUsersQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchKeyword = searchKeyword
        };

    private static UserDto FakeUserDto(
        string userName = "john",
        string email = "john@test.com") => new UserDto
        {
            Id = 1,
            UserName = userName,
            Email = email,
            Role = AppRoleEnum.Customer.ToString(),
            Status = UserStatusEnum.Active.ToString()
        };

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnPaginatedUsers()
    {
        var items = new List<UserDto>
        {
            FakeUserDto("alice", "alice@test.com"),
            FakeUserDto("bob",   "bob@test.com")
        };
        SetupPagingResult(items);

        var result = await _handler.Handle(BuildQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NoUsers()
    {
        SetupPagingResult(new List<UserDto>());

        var result = await _handler.Handle(BuildQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_PassCorrectPagingParams()
    {
        SetupPagingResult(new List<UserDto>(), page: 3, size: 5);

        var result = await _handler.Handle(BuildQuery(page: 3, pageSize: 5), default);

        result.IsSuccess.Should().BeTrue();
        _userRepository.Verify(r => r.GetPagingListAsync(
            It.IsAny<Expression<Func<ApplicationUser, UserDto>>>(),
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(),
            It.IsAny<List<string>>(),
            3,
            5,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_FilterBySearchKeyword_When_Provided()
    {
        SetupPagingResult(new List<UserDto> { FakeUserDto("rose", "rose@test.com") });

        var result = await _handler.Handle(BuildQuery(searchKeyword: "rose"), default);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.First().UserName.Should().Contain("rose");
    }
}
