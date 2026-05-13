using FlowerEcommerce.Application.Handlers.Users.Commands;

namespace FlowerEcommerce.Test.Handlers.Users.Commands;

public class UpdateUserStatusCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly ILogger<UpdateUserStatusCommandHandler> _logger;
    private readonly UpdateUserStatusCommandHandler _handler;

    private const ulong FakeUserId = 1;

    public UpdateUserStatusCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _logger = NullLogger<UpdateUserStatusCommandHandler>.Instance;

        _handler = new UpdateUserStatusCommandHandler(
            _userRepository.Object,
            _logger);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private UpdateUserStatusCommand BuildCommand(
        ulong userId = FakeUserId,
        UserStatusEnum status = UserStatusEnum.Inactive) => new UpdateUserStatusCommand
        {
            UserId = userId,
            Status = status
        };

    private void SetupUserRepo(ApplicationUser? user) =>
        _userRepository
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

    private static ApplicationUser FakeUser(
        ulong id = FakeUserId,
        AppRoleEnum role = AppRoleEnum.Customer) => new ApplicationUser
        {
            Id = id,
            Role = role,
            Status = UserStatusEnum.Active
        };

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        SetupUserRepo(null);

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsAdmin()
    {
        SetupUserRepo(FakeUser(role: AppRoleEnum.Administrator));

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
        _userRepository.Verify(r => r.Update(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_UpdateStatus()
    {
        var user = FakeUser();
        SetupUserRepo(user);

        var result = await _handler.Handle(
            BuildCommand(status: UserStatusEnum.Inactive), default);

        result.IsSuccess.Should().BeTrue();
        user.Status.Should().Be(UserStatusEnum.Inactive);
        _userRepository.Verify(r => r.Update(user), Times.Once);
        _userRepository.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_UpdateToCorrectStatus()
    {
        var user = FakeUser();
        SetupUserRepo(user);

        await _handler.Handle(BuildCommand(status: UserStatusEnum.Inactive), default);

        user.Status.Should().Be(UserStatusEnum.Inactive);
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        _userRepository
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
    }
}
