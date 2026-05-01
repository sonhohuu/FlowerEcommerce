namespace FlowerEcommerce.Application.Handlers.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator() 
        {
            RuleFor(x => x.Name)
            .Length(3, 50).WithMessage(MessageKey.CategoryNameValidLength);
        }
    }
}
