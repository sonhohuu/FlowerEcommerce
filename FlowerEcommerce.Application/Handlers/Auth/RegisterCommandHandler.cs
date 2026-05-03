using FlowerEcommerce.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.Application.Handlers.Auth;

public class RegisterCommand : IRequest<TResult>
{
    [Required]
    [MaxLength(AppConstants.MaxCommonLength)]
    public required string Username { get; set; }

    [Required]
    [MaxLength(AppConstants.MaxCommonLength)]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MaxLength(AppConstants.MaxCommonLength)]
    public required string Password { get; set; }

    [MaxLength(AppConstants.MaxCommonLength)]
    public string? FirstName { get; set; }

    [MaxLength(AppConstants.MaxCommonLength)]
    public string? LastName { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, TResult>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<TResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dbUser = await _userRepository.FirstOrDefaultAsync(
            predicate: x => request.Username.Equals(x.UserName) || request.Email.Equals(x.Email),
            asNoTracking: true,
            cancellationToken: cancellationToken
        );

        if (dbUser != null)
        {
            if (string.Equals(request.Username, dbUser.UserName, StringComparison.OrdinalIgnoreCase))
                return TResult.Failure("Username already exists.");

            if (string.Equals(request.Email, dbUser.Email, StringComparison.OrdinalIgnoreCase))
                return TResult.Failure("Email already exists.");

            return TResult.Failure("User already exists.");
        }

        var newUser = new ApplicationUser
        {
            UserName = request.Username.ToLower(),
            Email = request.Email.ToLower(),
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var hasher = new PasswordHasher<ApplicationUser>();
        newUser.PasswordHash = hasher.HashPassword(newUser, request.Password);

        _userRepository.Add(newUser);

        await _userRepository.SaveChangesAsync(cancellationToken);

        return TResult.Success();
    }
}
