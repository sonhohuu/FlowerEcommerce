using FlowerEcommerce.Application.Common.Constants;
using FlowerEcommerce.Application.Handlers.Auth;
using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.Test.Handlers.Auth;

public class RegisterCommandTests
{
    private static IList<ValidationResult> Validate(RegisterCommand command)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(command);
        Validator.TryValidateObject(command, context, results, validateAllProperties: true);
        return results;
    }

    private static RegisterCommand ValidCommand() => new()
    {
        Username = "testuser",
        Email = "test@example.com",
        Password = "password123"
    };

    // ── Username ──
    [Fact]
    public void Username_Valid_ShouldPass()
    {
        var command = ValidCommand();
        Validate(command).Should().BeEmpty();
    }

    [Fact]
    public void Username_TooLong_ShouldFail()
    {
        var command = ValidCommand();
        command.Username = new string('A', AppConstants.MaxCommonLength + 1);
        Validate(command).Should().Contain(x => x.MemberNames.Contains(nameof(RegisterCommand.Username)));
    }

    [Fact]
    public void Username_ExactMaxLength_ShouldPass()
    {
        var command = ValidCommand();
        command.Username = new string('A', AppConstants.MaxCommonLength);
        Validate(command).Should().BeEmpty();
    }

    // ── Email ──
    [Fact]
    public void Email_Valid_ShouldPass()
    {
        var command = ValidCommand();
        Validate(command).Should().BeEmpty();
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public void Email_InvalidFormat_ShouldFail(string email)
    {
        var command = ValidCommand();
        command.Email = email;
        Validate(command).Should().Contain(x => x.MemberNames.Contains(nameof(RegisterCommand.Email)));
    }

    [Fact]
    public void Email_TooLong_ShouldFail()
    {
        var command = ValidCommand();
        command.Email = new string('A', AppConstants.MaxCommonLength) + "@test.com";
        Validate(command).Should().Contain(x => x.MemberNames.Contains(nameof(RegisterCommand.Email)));
    }

    // ── Password ──
    [Fact]
    public void Password_Valid_ShouldPass()
    {
        var command = ValidCommand();
        Validate(command).Should().BeEmpty();
    }

    [Fact]
    public void Password_TooLong_ShouldFail()
    {
        var command = ValidCommand();
        command.Password = new string('A', AppConstants.MaxCommonLength + 1);
        Validate(command).Should().Contain(x => x.MemberNames.Contains(nameof(RegisterCommand.Password)));
    }

    // ── FirstName ──
    [Fact]
    public void FirstName_Null_ShouldPass()
    {
        var command = ValidCommand();
        command.FirstName = null;
        Validate(command).Should().BeEmpty();
    }

    [Fact]
    public void FirstName_Valid_ShouldPass()
    {
        var command = ValidCommand();
        command.FirstName = "Nguyen";
        Validate(command).Should().BeEmpty();
    }

    [Fact]
    public void FirstName_TooLong_ShouldFail()
    {
        var command = ValidCommand();
        command.FirstName = new string('A', AppConstants.MaxCommonLength + 1);
        Validate(command).Should().Contain(x => x.MemberNames.Contains(nameof(RegisterCommand.FirstName)));
    }

    // ── LastName ──
    [Fact]
    public void LastName_Null_ShouldPass()
    {
        var command = ValidCommand();
        command.LastName = null;
        Validate(command).Should().BeEmpty();
    }

    [Fact]
    public void LastName_Valid_ShouldPass()
    {
        var command = ValidCommand();
        command.LastName = "Van A";
        Validate(command).Should().BeEmpty();
    }

    [Fact]
    public void LastName_TooLong_ShouldFail()
    {
        var command = ValidCommand();
        command.LastName = new string('A', AppConstants.MaxCommonLength + 1);
        Validate(command).Should().Contain(x => x.MemberNames.Contains(nameof(RegisterCommand.LastName)));
    }
}
