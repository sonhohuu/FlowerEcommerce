namespace FlowerEcommerce.Application.Handlers.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage(MessageKey.ProductNameValidLength)
            .When(x => x.Name != null);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage(MessageKey.ProductDescriptionValidLength)
            .When(x => x.Description != null);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage(MessageKey.ProductPriceValid)
            .When(x => x.Price != null);
    }   
}
