using FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;
using FlowerEcommerce.Application.Handlers.Orders.Commands.UpdateOrder;

namespace FlowerEcommerce.Test.Handlers.Orders.Commands;

public class UpdateOrderCommandValidatorTests
{
    private readonly UpdateOrderCommandValidator _validator = new();

    private static UpdateOrderCommand ValidCommand() => new()
    {
        Id = 1
    };

    private static OrderItemDto ValidItem() => new()
    {
        ProductId = 1,
        Quantity = 1,
        Price = 50000
    };

    // ── CustomerName ──
    [Fact]
    public void CustomerName_Null_ShouldPass()
    {
        var command = ValidCommand() with { CustomerName = null };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.CustomerName);
    }

    [Theory]
    [InlineData("ABCDE")]
    [InlineData("Nguyen Van A")]
    public void CustomerName_Valid_ShouldPass(string name)
    {
        var command = ValidCommand() with { CustomerName = name };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.CustomerName);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("ABCD")]
    public void CustomerName_TooShort_ShouldFail(string name)
    {
        var command = ValidCommand() with { CustomerName = name };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.CustomerName)
                  .WithErrorMessage(MessageKey.CustomerNameValidLength);
    }

    [Fact]
    public void CustomerName_TooLong_ShouldFail()
    {
        var command = ValidCommand() with { CustomerName = new string('A', 101) };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.CustomerName)
                  .WithErrorMessage(MessageKey.CustomerNameValidLength);
    }

    // ── PhoneNumber ──
    [Fact]
    public void PhoneNumber_Null_ShouldPass()
    {
        var command = ValidCommand() with { PhoneNumber = null };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("0912345678")]
    [InlineData("0123456789")]
    public void PhoneNumber_Valid_ShouldPass(string phone)
    {
        var command = ValidCommand() with { PhoneNumber = phone };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("091234567")]    // 9 digits
    [InlineData("09123456789")]  // 11 digits
    [InlineData("091234567a")]   // contains letter
    [InlineData("0912 34567")]   // contains space
    public void PhoneNumber_InvalidFormat_ShouldFail(string phone)
    {
        var command = ValidCommand() with { PhoneNumber = phone };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.PhoneNumber)
                  .WithErrorMessage(MessageKey.PhoneNumberValid);
    }

    // ── Address ──
    [Fact]
    public void Address_Null_ShouldPass()
    {
        var command = ValidCommand() with { Address = null };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Address_Valid_ShouldPass()
    {
        var command = ValidCommand() with { Address = "123 Đường Lê Lợi, Quận 1" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Address_TooShort_ShouldFail()
    {
        var command = ValidCommand() with { Address = "123 ABC" }; // 7 chars
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Address)
                  .WithErrorMessage(MessageKey.AddressValidLength);
    }

    [Fact]
    public void Address_TooLong_ShouldFail()
    {
        var command = ValidCommand() with { Address = new string('A', 201) };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Address)
                  .WithErrorMessage(MessageKey.AddressValidLength);
    }

    [Fact]
    public void Address_ExactMinLength_ShouldPass()
    {
        var command = ValidCommand() with { Address = new string('A', 10) };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    // ── Items ──
    [Fact]
    public void Items_Null_ShouldPass()
    {
        var command = ValidCommand() with { Items = null };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Items_Valid_ShouldPass()
    {
        var command = ValidCommand() with { Items = [ValidItem()] };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Items);
    }

    // ── Items[].Quantity ──
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Item_Quantity_Valid_ShouldPass(int quantity)
    {
        var command = ValidCommand() with { Items = [ValidItem() with { Quantity = quantity }] };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor("Items[0].Quantity");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Item_Quantity_ZeroOrNegative_ShouldFail(int quantity)
    {
        var command = ValidCommand() with { Items = [ValidItem() with { Quantity = quantity }] };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor("Items[0].Quantity")
                  .WithErrorMessage(MessageKey.OrderItemQuantity);
    }

    // ── Items[].Price ──
    [Theory]
    [InlineData(0)]
    [InlineData(50000)]
    public void Item_Price_Valid_ShouldPass(decimal price)
    {
        var command = ValidCommand() with { Items = [ValidItem() with { Price = price }] };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor("Items[0].Price");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-9999)]
    public void Item_Price_Negative_ShouldFail(decimal price)
    {
        var command = ValidCommand() with { Items = [ValidItem() with { Price = price }] };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor("Items[0].Price")
                  .WithErrorMessage(MessageKey.ProductPriceValid);
    }
}
