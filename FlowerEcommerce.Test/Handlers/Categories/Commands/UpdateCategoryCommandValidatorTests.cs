using FlowerEcommerce.Application.Handlers.Categories.Commands.UpdateCategory;

namespace FlowerEcommerce.Test.Handlers.Categories.Commands;

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    [Fact]
    public void Name_Valid_ShouldPass()
    {
        var command = new UpdateCategoryCommand { Name = "Hoa tươi" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("AB")]
    public void Name_TooShort_ShouldFail(string name)
    {
        var command = new UpdateCategoryCommand { Name = name };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage(MessageKey.CategoryNameValidLength);
    }

    [Fact]
    public void Name_TooLong_ShouldFail()
    {
        var command = new UpdateCategoryCommand { Name = new string('A', 51) };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage(MessageKey.CategoryNameValidLength);
    }

    [Theory]
    [InlineData("ABC")]
    [InlineData("Hoa lan")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWX")]
    public void Name_BoundaryValid_ShouldPass(string name)
    {
        var command = new UpdateCategoryCommand { Name = name };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
