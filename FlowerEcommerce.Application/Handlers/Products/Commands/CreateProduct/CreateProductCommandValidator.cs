namespace FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator() 
        {
            RuleFor(x => x.Name)
                .Length(5, 100).WithMessage(MessageKey.ProductNameValidLength);

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage(MessageKey.ProductDescriptionValidLength);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage(MessageKey.ProductPriceValid);
        }
    }
}
