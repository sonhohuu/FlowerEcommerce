namespace FlowerEcommerce.Application.Handlers.ProductRatings.Commands.UpsertProductRating;

public class UpsertProductRatingCommandHandler : IRequestHandler<UpsertProductRatingCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpsertProductRatingCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public UpsertProductRatingCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpsertProductRatingCommandHandler> logger,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResult> Handle(
        UpsertProductRatingCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.UserId;

            // 1. Kiểm tra sản phẩm tồn tại
            var productExists = await _unitOfWork.Repository<Product>()
                .AnyAsync(
                    predicate: p => p.Id == request.ProductId,
                    cancellationToken: cancellationToken);

            if (!productExists)
            {
                _logger.LogWarning("Product {ProductId} not found", request.ProductId);
                return TResult.Failure(MessageKey.ProductNotFound, ErrorCodes.NOT_FOUND);
            }

            // 2. Tìm rating cũ của user cho sản phẩm này
            var ratingRepo = _unitOfWork.Repository<ProductRating>();
            var existingRating = await ratingRepo.FirstOrDefaultAsync(
                predicate: r => r.ProductId == request.ProductId
                             && r.UserId == currentUserId,
                cancellationToken: cancellationToken);

            if (existingRating is null)
            {
                // ── CREATE ──
                var newRating = request.Adapt<ProductRating>();
                newRating.UserId = currentUserId;
                ratingRepo.Add(newRating);

                _logger.LogInformation(
                    "Created Rating for Product {ProductId} by User {UserId}",
                    request.ProductId, currentUserId);
            }
            else
            {
                // ── UPDATE ──
                existingRating.Score = request.Score;
                existingRating.Comment = string.IsNullOrWhiteSpace(request.Comment)
                    ? existingRating.Comment
                    : request.Comment.Trim();

                ratingRepo.Update(existingRating);

                _logger.LogInformation(
                    "Updated Rating {RatingId} for Product {ProductId} by User {UserId}",
                    existingRating.Id, request.ProductId, currentUserId);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upsert Product Rating Failed for Product {ProductId}", request.ProductId);
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
