using FlowerEcommerce.Application.Common.Constants;
using FlowerEcommerce.Application.Handlers.Auth;
using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.Test.Handlers.Auth;

public class LoginCommandTests
{
    private static IList<ValidationResult> Validate(LoginCommand command)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(command);
        Validator.TryValidateObject(command, context, results, validateAllProperties: true);
        return results;
    }

    private static LoginCommand ValidCommand() => new()
    {
        Username = "testuser",
        Password = "password123"
    };

    // ── Username ──
    [Fact]
    public void Username_Valid_ShouldPass()
    {
        Validate(ValidCommand()).Should().BeEmpty();
    }

    [Fact]
    public void Username_ExactMaxLength_ShouldPass()
    {
        var command = ValidCommand();
        command.Username = new string('A', AppConstants.MaxCommonLength);
        Validate(command).Should().BeEmpty();
    }

    [Fact]
    public void Username_TooLong_ShouldFail()
    {
        var command = ValidCommand();
        command.Username = new string('A', AppConstants.MaxCommonLength + 1);
        Validate(command).Should().Contain(x => x.MemberNames.Contains(nameof(LoginCommand.Username)));
    }

    // ── Password ──
    [Fact]
    public void Password_Valid_ShouldPass()
    {
        Validate(ValidCommand()).Should().BeEmpty();
    }

    [Fact]
    public void Password_ExactMaxLength_ShouldPass()
    {
        var command = ValidCommand();
        command.Password = new string('A', AppConstants.MaxCommonLength);
        Validate(command).Should().BeEmpty();
    }

    [Fact]
    public void Password_TooLong_ShouldFail()
    {
        var command = ValidCommand();
        command.Password = new string('A', AppConstants.MaxCommonLength + 1);
        Validate(command).Should().Contain(x => x.MemberNames.Contains(nameof(LoginCommand.Password)));
    }
}
