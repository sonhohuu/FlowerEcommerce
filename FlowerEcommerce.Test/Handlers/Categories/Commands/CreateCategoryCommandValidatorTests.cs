using FlowerEcommerce.Application.Handlers.Categories.Commands.CreateCategory;


namespace FlowerEcommerce.Test.Handlers.Categories.Commands;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Name_Valid_ShouldPass()
    {
        var command = new CreateCategoryCommand { Name = "Hoa tươi" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]        // rỗng
    [InlineData("AB")]      // 2 ký tự — dưới min
    [InlineData("A")]       // 1 ký tự
    public void Name_TooShort_ShouldFail(string name)
    {
        var command = new CreateCategoryCommand { Name = name };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage(MessageKey.CategoryNameValidLength);
    }

    [Fact]
    public void Name_TooLong_ShouldFail()
    {
        var command = new CreateCategoryCommand { Name = new string('A', 51) };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage(MessageKey.CategoryNameValidLength);
    }

    [Theory]
    [InlineData("ABC")]                  // đúng min (3)
    [InlineData("ABCDE")]               // giữa
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWX")] // đúng max (50)
    public void Name_BoundaryValid_ShouldPass(string name)
    {
        var command = new CreateCategoryCommand { Name = name };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
