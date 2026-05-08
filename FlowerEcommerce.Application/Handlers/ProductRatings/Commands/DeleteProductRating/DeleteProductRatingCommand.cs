namespace FlowerEcommerce.Application.Handlers.ProductRatings.Commands.DeleteProductRating
{
    public record DeleteProductRatingCommand : IRequest<TResult>
    {
        public required ulong Id { get; init; }
    }
}
