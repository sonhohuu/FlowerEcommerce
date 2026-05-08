namespace FlowerEcommerce.Application.Handlers.ProductRatings.Commands.UpsertProductRating;

public class UpsertProductRatingCommand : IRequest<TResult>
{
    public ulong ProductId { get; init; }
    public int Score { get; init; }
    public string? Comment { get; init; }
}
