namespace FlowerEcommerce.Application.Handlers.ProductRatings.Queries.GetProductRatings;

public class GetProductRatingsQueryHandler
    : IRequestHandler<GetProductRatingsQuery, TResult<IPaginate<GetProductRatingsQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetProductRatingsQueryHandler> _logger;

    public GetProductRatingsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetProductRatingsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult<IPaginate<GetProductRatingsQueryResponse>>> Handle(
        GetProductRatingsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ratings = await _unitOfWork.Repository<ProductRating>()
            .GetPagingListAsync(
                predicate: r => r.ProductId == request.ProductId,
                includes: [nameof(ProductRating.User)],
                selector: pr => new GetProductRatingsQueryResponse
                {
                    Id = pr.Id,
                    Score = pr.Score,
                    Comment = pr.Comment,
                    UserId = pr.UserId,
                    UserName = pr.User.FirstName + " " + pr.User.LastName
                },
                page: request.Page,
                size: request.PageSize,
                cancellationToken: cancellationToken);

            return TResult<IPaginate<GetProductRatingsQueryResponse>>
                .Success(ratings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get Product Ratings Failed");
            return TResult<IPaginate<GetProductRatingsQueryResponse>>
                .Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }

    }
}