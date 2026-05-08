using FlowerEcommerce.Application.Handlers.Products.Queries.GetProducts;

namespace FlowerEcommerce.Application.Handlers.ProductRatings.Queries.GetProductRatings;

public class GetProductRatingsQueryHandler
    : IRequestHandler<GetProductRatingsQuery, TResult<IPaginate<GetProductRatingsQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductRatingsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;   
    }

    public async Task<TResult<IPaginate<GetProductRatingsQueryResponse>>> Handle(
        GetProductRatingsQuery request,
        CancellationToken cancellationToken)
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
}