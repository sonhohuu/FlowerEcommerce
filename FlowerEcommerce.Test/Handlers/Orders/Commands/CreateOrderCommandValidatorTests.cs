using FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;

namespace FlowerEcommerce.Test.Handlers.Orders.Commands;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    private static CreateOrderCommand ValidCommand() => new()
    {
        CustomerName = "Nguyen Van A",
        PhoneNumber = "0912345678",
        Address = "123 Đường Lê Lợi, Quận 1, TP.HCM",
        Items = [new OrderItemDto { ProductId = 1, Quantity = 1, Price = 50000 }]
    };

    private static OrderItemDto ValidItem() => new()
    {
        ProductId = 1,
        Quantity = 1,
        Price = 50000
    };

    // ── CustomerName ──
    [Fact]
    public void CustomerName_Valid_ShouldPass()
    {
        _validator.TestValidate(ValidCommand()).ShouldNotHaveValidationErrorFor(x => x.CustomerName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CustomerName_Empty_ShouldFail(string name)
    {
        var command = ValidCommand() with { CustomerName = name };
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.CustomerName);
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

    [Theory]
    [InlineData("ABCDE")]         // min = 5
    [InlineData("Nguyen Van A")]
    public void CustomerName_BoundaryValid_ShouldPass(string name)
    {
        var command = ValidCommand() with { CustomerName = name };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.CustomerName);
    }

    // ── PhoneNumber ──
    [Fact]
    public void PhoneNumber_Valid_ShouldPass()
    {
        _validator.TestValidate(ValidCommand()).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void PhoneNumber_Empty_ShouldFail(string phone)
    {
        var command = ValidCommand() with { PhoneNumber = phone };
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.PhoneNumber);
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

    [Theory]
    [InlineData("0912345678")]
    [InlineData("0123456789")]
    public void PhoneNumber_ExactTenDigits_ShouldPass(string phone)
    {
        var command = ValidCommand() with { PhoneNumber = phone };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    // ── Address ──
    [Fact]
    public void Address_Valid_ShouldPass()
    {
        _validator.TestValidate(ValidCommand()).ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Address_Empty_ShouldFail(string address)
    {
        var command = ValidCommand() with { Address = address };
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Address);
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
    public void Items_Valid_ShouldPass()
    {
        _validator.TestValidate(ValidCommand()).ShouldNotHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Items_Empty_ShouldFail()
    {
        var command = ValidCommand() with { Items = [] };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Items)
                  .WithErrorMessage(MessageKey.OrderItemsRequired);
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
