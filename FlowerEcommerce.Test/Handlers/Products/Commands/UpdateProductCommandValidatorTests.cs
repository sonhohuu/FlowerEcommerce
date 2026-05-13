using FlowerEcommerce.Application.Handlers.Products.Commands.UpdateProduct;

namespace FlowerEcommerce.Test.Handlers.Products.Commands;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator = new();

    // ── Name ──
    [Fact]
    public void Name_Null_ShouldPass()
    {
        var command = new UpdateProductCommand { Id = 1, Name = null };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_Valid_ShouldPass()
    {
        var command = new UpdateProductCommand { Id = 1, Name = "Hoa hồng đỏ" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_TooLong_ShouldFail()
    {
        var command = new UpdateProductCommand { Id = 1, Name = new string('A', 101) };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage(MessageKey.ProductNameValidLength);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Hoa lan trắng")]
    [InlineData("ABCDEFGHIJ")]
    public void Name_BoundaryValid_ShouldPass(string name)
    {
        var command = new UpdateProductCommand { Id = 1, Name = name };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    // ── Description ──
    [Fact]
    public void Description_Null_ShouldPass()
    {
        var command = new UpdateProductCommand { Id = 1, Description = null };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_Valid_ShouldPass()
    {
        var command = new UpdateProductCommand { Id = 1, Description = "Mô tả ngắn" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_TooLong_ShouldFail()
    {
        var command = new UpdateProductCommand { Id = 1, Description = new string('A', 501) };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage(MessageKey.ProductDescriptionValidLength);
    }

    [Fact]
    public void Description_ExactMaxLength_ShouldPass()
    {
        var command = new UpdateProductCommand { Id = 1, Description = new string('A', 500) };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    // ── Price ──
    [Fact]
    public void Price_Null_ShouldPass()
    {
        var command = new UpdateProductCommand { Id = 1, Price = null };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(50000)]
    public void Price_Valid_ShouldPass(decimal price)
    {
        var command = new UpdateProductCommand { Id = 1, Price = price };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-9999)]
    public void Price_Negative_ShouldFail(decimal price)
    {
        var command = new UpdateProductCommand { Id = 1, Price = price };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Price)
                  .WithErrorMessage(MessageKey.ProductPriceValid);
    }
}
