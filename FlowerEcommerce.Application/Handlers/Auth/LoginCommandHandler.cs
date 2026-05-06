using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.Application.Handlers.Auth;

public class LoginCommand : IRequest<TResult<LoginResponse>>
{
    [Required]
    [MaxLength(AppConstants.MaxCommonLength)]
    public required string Username { get; set; }

    [Required]
    [MaxLength(AppConstants.MaxCommonLength)]
    public required string Password { get; set; }
}

public class LoginResponse
{
    public JwtResponse? TokenModel { get; set; }
    public ulong UserId { get; set; }
    public string? Username { get; set; }
    public required string AllowedRole { get; set; }
}
public class LoginCommandHandler : IRequestHandler<LoginCommand, TResult<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService, UserManager<ApplicationUser> userManager)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
    }

    public async Task<TResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            predicate: x => x.Status == UserStatusEnum.Active &&
                 (
                     request.Username.Equals(x.UserName) ||
                     request.Username.Equals(x.Email) 
                 ),
            asNoTracking: true,
            cancellationToken: cancellationToken
        );

        if (user == null) return TResult<LoginResponse>.Failure("Incorrect username or password.");

        if (user.Status == UserStatusEnum.Inactive) return TResult<LoginResponse>.Failure("User was inactive.");

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return TResult<LoginResponse>.Failure("Incorrect username or password.");

        var jwtTokenResponse = await _jwtTokenService.GetJwtToken(user.Id, cancellationToken);

        var jwtToken = new JwtResponse();

        if (jwtTokenResponse.IsSuccess)
        {
            jwtToken = jwtTokenResponse.Data!;
        }

        string currentRoleName = user.Role switch
        {
            AppRoleEnum.Administrator => "Administrator",
            AppRoleEnum.Customer => "Customer",
            _ => "Customer"
        };

        var loginResponse = new LoginResponse
        {
            TokenModel = jwtToken,
            UserId = user.Id,
            Username = user.UserName,
            AllowedRole = currentRoleName
        };

        return TResult<LoginResponse>.Success(loginResponse);

    }
}
