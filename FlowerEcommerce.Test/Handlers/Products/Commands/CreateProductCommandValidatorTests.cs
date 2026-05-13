using FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

namespace FlowerEcommerce.Test.Handlers.Products.Commands;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    // ── Name ──
    [Fact]
    public void Name_Valid_ShouldPass()
    {
        var command = new CreateProductCommand { Name = "Hoa hồng đỏ", Price = 50000, Description = "Mô tả" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("ABCD")]
    public void Name_TooShort_ShouldFail(string name)
    {
        var command = new CreateProductCommand { Name = name, Price = 50000, Description = "Mô tả" };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage(MessageKey.ProductNameValidLength);
    }

    [Fact]
    public void Name_TooLong_ShouldFail()
    {
        var command = new CreateProductCommand { Name = new string('A', 101), Price = 50000, Description = "Mô tả" };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage(MessageKey.ProductNameValidLength);
    }

    [Theory]
    [InlineData("ABCDE")]
    [InlineData("Hoa lan trắng")]
    public void Name_BoundaryValid_ShouldPass(string name)
    {
        var command = new CreateProductCommand { Name = name, Price = 50000, Description = "Mô tả" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    // ── Description ──
    [Fact]
    public void Description_Valid_ShouldPass()
    {
        var command = new CreateProductCommand { Name = "Hoa hồng", Price = 50000, Description = "Mô tả ngắn" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_TooLong_ShouldFail()
    {
        var command = new CreateProductCommand { Name = "Hoa hồng", Price = 50000, Description = new string('A', 501) };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage(MessageKey.ProductDescriptionValidLength);
    }

    // ── Price ──
    [Theory]
    [InlineData(1)]
    [InlineData(50000)]
    [InlineData(999999)]
    public void Price_Valid_ShouldPass(decimal price)
    {
        var command = new CreateProductCommand { Name = "Hoa hồng", Price = price, Description = "Mô tả" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-9999)]
    public void Price_ZeroOrNegative_ShouldFail(decimal price)
    {
        var command = new CreateProductCommand { Name = "Hoa hồng", Price = price, Description = "Mô tả" };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Price)
                  .WithErrorMessage(MessageKey.ProductPriceValid);
    }
}
