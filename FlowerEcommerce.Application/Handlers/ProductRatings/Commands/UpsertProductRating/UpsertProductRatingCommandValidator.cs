namespace FlowerEcommerce.Application.Handlers.ProductRatings.Commands.UpsertProductRating;

public class UpsertProductRatingCommandValidator : AbstractValidator<UpsertProductRatingCommand>
{
    public UpsertProductRatingCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0UL)
            .WithMessage("ProductId không hợp lệ.");

        RuleFor(x => x.Score)
            .NotNull().WithMessage("Điểm đánh giá không được để trống.")
            .InclusiveBetween(1, 5).WithMessage(MessageKey.ProductRatingScoreValid);

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Nội dung đánh giá không được để trống.")
            .MaximumLength(1000).WithMessage(MessageKey.ProductRatingCommentValidLength);
    }
}
