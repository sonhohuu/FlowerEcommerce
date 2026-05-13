using FlowerEcommerce.Application.Handlers.Auth;

namespace FlowerEcommerce.Test.Handlers.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IJwtTokenService> _jwtTokenService;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _jwtTokenService = new Mock<IJwtTokenService>();
        _handler = new RefreshTokenCommandHandler(_jwtTokenService.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private static RefreshTokenCommand BuildCommand(Guid? token = null) =>
        new RefreshTokenCommand { RefreshToken = token ?? Guid.NewGuid() };

    private void SetupRefreshJwtToken(TResult<JwtResponse> returns) =>
        _jwtTokenService.Setup(s => s.RefreshJwtToken(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(returns);

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RefreshTokenValid()
    {
        // Arrange
        SetupRefreshJwtToken(TResult<JwtResponse>.Success(new JwtResponse
        {
            AccessToken = "new-access-token",
            RefreshToken = Guid.NewGuid()
        }));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("new-access-token");
        result.Data.RefreshToken.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RefreshTokenInvalid()
    {
        // Arrange
        SetupRefreshJwtToken(TResult<JwtResponse>.Failure("Invalid refresh token."));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_PassCorrectToken_To_JwtService()
    {
        // Arrange
        var token = Guid.NewGuid();
        Guid? capturedToken = null;

        _jwtTokenService.Setup(s => s.RefreshJwtToken(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .Callback<Guid, CancellationToken>((t, _) => capturedToken = t)
        .ReturnsAsync(TResult<JwtResponse>.Success(new JwtResponse
        {
            AccessToken = "new-access-token",
            RefreshToken = Guid.NewGuid()
        }));

        // Act
        await _handler.Handle(BuildCommand(token), default);

        // Assert
        capturedToken.Should().Be(token);
    }
}
