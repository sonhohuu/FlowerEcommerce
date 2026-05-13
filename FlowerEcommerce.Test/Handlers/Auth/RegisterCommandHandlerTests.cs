using FlowerEcommerce.Application.Handlers.Auth;

namespace FlowerEcommerce.Test.Handlers.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _handler = new RegisterCommandHandler(_userRepository.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static RegisterCommand BuildCommand(
        string username = "testuser",
        string email = "test@example.com",
        string password = "Test@123") =>
        new RegisterCommand
        {
            Username = username,
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "User"
        };

    private static ApplicationUser FakeUser(
        string username = "testuser",
        string email = "test@example.com") =>
        new ApplicationUser
        {
            UserName = username,
            Email = email,
            Status = UserStatusEnum.Active
        };

    private void SetupFirstOrDefault(ApplicationUser? returns) =>
        _userRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UsernameAlreadyExists()
    {
        // Arrange
        SetupFirstOrDefault(FakeUser(username: "testuser"));

        // Act
        var result = await _handler.Handle(BuildCommand(username: "testuser"), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EmailAlreadyExists()
    {
        // Arrange
        SetupFirstOrDefault(FakeUser(username: "otheruser", email: "test@example.com"));

        // Act
        var result = await _handler.Handle(BuildCommand(email: "test@example.com"), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NewUser()
    {
        // Arrange
        SetupFirstOrDefault(null);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepository.Verify(r => r.Add(It.IsAny<ApplicationUser>()), Times.Once);
        _userRepository.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_LowercaseUsernameAndEmail_When_Registering()
    {
        // Arrange
        ApplicationUser? savedUser = null;
        SetupFirstOrDefault(null);
        _userRepository
            .Setup(r => r.Add(It.IsAny<ApplicationUser>()))
            .Callback<ApplicationUser>(u => savedUser = u);

        // Act
        await _handler.Handle(BuildCommand(username: "TestUser", email: "Test@Example.COM"), default);

        // Assert
        savedUser.Should().NotBeNull();
        savedUser!.UserName.Should().Be("testuser");
        savedUser.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Handle_Should_HashPassword_When_Registering()
    {
        // Arrange
        ApplicationUser? savedUser = null;
        SetupFirstOrDefault(null);
        _userRepository
            .Setup(r => r.Add(It.IsAny<ApplicationUser>()))
            .Callback<ApplicationUser>(u => savedUser = u);

        // Act
        await _handler.Handle(BuildCommand(password: "Test@123"), default);

        // Assert
        savedUser.Should().NotBeNull();
        savedUser!.PasswordHash.Should().NotBeNullOrEmpty();
        savedUser.PasswordHash.Should().NotBe("Test@123");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserAlreadyExists_ButNoExactMatch()
    {
        // Arrange — dbUser != null nhưng username/email không khớp chính xác
        SetupFirstOrDefault(new ApplicationUser
        {
            UserName = "differentuser",
            Email = "different@example.com"
        });

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
