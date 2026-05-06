namespace FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .Length(5, 100).WithMessage(MessageKey.CustomerNameValidLength);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\d{10}$").WithMessage(MessageKey.PhoneNumberValid);
        RuleFor(x => x.Address)
            .NotEmpty()
            .Length(10, 200).WithMessage(MessageKey.AddressValidLength);
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage(MessageKey.OrderItemsRequired);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage(MessageKey.OrderItemQuantity);

            item.RuleFor(i => i.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage(MessageKey.ProductPriceValid);
        });
    }
}
