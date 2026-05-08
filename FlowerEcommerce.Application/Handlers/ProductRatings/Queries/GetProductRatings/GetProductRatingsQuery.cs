using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.Application.Handlers.ProductRatings.Queries.GetProductRatings;

public class GetProductRatingsQuery : PaginationRequest, IRequest<TResult<IPaginate<GetProductRatingsQueryResponse>>>
{
    [Required]
    public required ulong ProductId { get; init; }
}

public class GetProductRatingsQueryResponse
{
    public ulong Id { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; }
    public ulong? UserId { get; set; }
    public string? UserName { get; set; }
}

