namespace FlowerEcommerce.Application.Handlers.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .Length(3, 50).WithMessage(MessageKey.CategoryNameValidLength);
    }
}
