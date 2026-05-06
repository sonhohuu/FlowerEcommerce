namespace FlowerEcommerce.Application.Handlers.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .Length(5, 100).WithMessage(MessageKey.CustomerNameValidLength)
            .When(x => x != null);
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d{10}$").WithMessage(MessageKey.PhoneNumberValid)
            .When(x => x != null);
        RuleFor(x => x.Address)
            .Length(10, 200).WithMessage(MessageKey.AddressValidLength)
            .When(x => x != null);
        When(x => x.Items != null, () =>
        {
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage(MessageKey.OrderItemQuantity);
                item.RuleFor(i => i.Price)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage(MessageKey.ProductPriceValid);
            });
        });
    }
}
