using FlowerEcommerce.Application.Handlers.Auth;
using Microsoft.AspNetCore.Identity;

namespace FlowerEcommerce.Test.Handlers.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IJwtTokenService> _jwtTokenService;
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _jwtTokenService = new Mock<IJwtTokenService>();
        _userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        _handler = new LoginCommandHandler(
            _userRepository.Object,
            _jwtTokenService.Object,
            _userManager.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static LoginCommand BuildCommand(
        string username = "testuser",
        string password = "Test@123") =>
        new LoginCommand { Username = username, Password = password };

    private static ApplicationUser FakeUser(
        ulong id = 1,
        AppRoleEnum role = AppRoleEnum.Customer) =>
        new ApplicationUser
        {
            Id = id,
            UserName = "testuser",
            Email = "test@example.com",
            Status = UserStatusEnum.Active,
            Role = role
        };

    private void SetupFirstOrDefault(ApplicationUser? returns) =>
        _userRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<List<string>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    private void SetupCheckPassword(bool returns) =>
        _userManager.Setup(m => m.CheckPasswordAsync(
            It.IsAny<ApplicationUser>(),
            It.IsAny<string>()))
        .ReturnsAsync(returns);

    private void SetupGetJwtToken(bool isSuccess) =>
        _jwtTokenService.Setup(s => s.GetJwtToken(
            It.IsAny<ulong>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(isSuccess
            ? TResult<JwtResponse>.Success(new JwtResponse
            {
                AccessToken = "access-token",
                RefreshToken = Guid.NewGuid()
            })
            : TResult<JwtResponse>.Failure("Token error"));

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        SetupFirstOrDefault(null);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PasswordIncorrect()
    {
        // Arrange
        SetupFirstOrDefault(FakeUser());
        SetupCheckPassword(false);

        // Act
        var result = await _handler.Handle(BuildCommand(password: "WrongPass"), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCredentials()
    {
        // Arrange
        SetupFirstOrDefault(FakeUser());
        SetupCheckPassword(true);
        SetupGetJwtToken(true);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Username.Should().Be("testuser");
        result.Data.TokenModel.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnCorrectRole_When_UserIsAdministrator()
    {
        // Arrange
        SetupFirstOrDefault(FakeUser(role: AppRoleEnum.Administrator));
        SetupCheckPassword(true);
        SetupGetJwtToken(true);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.AllowedRole.Should().Be("Administrator");
    }

    [Fact]
    public async Task Handle_Should_ReturnCorrectRole_When_UserIsCustomer()
    {
        // Arrange
        SetupFirstOrDefault(FakeUser(role: AppRoleEnum.Customer));
        SetupCheckPassword(true);
        SetupGetJwtToken(true);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.AllowedRole.Should().Be("Customer");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_EmptyToken_When_JwtFails()
    {
        // Arrange
        SetupFirstOrDefault(FakeUser());
        SetupCheckPassword(true);
        SetupGetJwtToken(false);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.TokenModel.Should().NotBeNull();
        result.Data.TokenModel!.AccessToken.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnCorrectUserId_When_LoginSuccess()
    {
        // Arrange
        SetupFirstOrDefault(FakeUser(id: 99));
        SetupCheckPassword(true);
        SetupGetJwtToken(true);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.UserId.Should().Be(99);
    }

    [Fact]
    public async Task Handle_Should_ReturnCustomerRole_When_RoleIsUnknown()
    {
        // Arrange
        SetupFirstOrDefault(FakeUser(role: (AppRoleEnum)999)); // role không tồn tại
        SetupCheckPassword(true);
        SetupGetJwtToken(true);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.AllowedRole.Should().Be("Customer");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsInactive()
    {
        // Arrange
        var inactiveUser = FakeUser();
        inactiveUser.Status = UserStatusEnum.Inactive;
        SetupFirstOrDefault(inactiveUser);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
