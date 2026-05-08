namespace FlowerEcommerce.Application.Handlers.ProductRatings.Commands.DeleteProductRating;

public class DeleteProductRatingCommandHandler
    : IRequestHandler<DeleteProductRatingCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProductRatingCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public DeleteProductRatingCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteProductRatingCommandHandler> logger,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResult> Handle(
        DeleteProductRatingCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ratingRepository = _unitOfWork.Repository<ProductRating>();

            var rating = await ratingRepository.FirstOrDefaultAsync(
                predicate: r => r.Id == request.Id,
                cancellationToken: cancellationToken);

            if (rating is null)
            {
                _logger.LogWarning(
                    "Product Rating {RatingId} not found",
                    request.Id);

                return TResult.Failure(
                    MessageKey.ProductRatingNotFound,
                    ErrorCodes.NOT_FOUND);
            }

            // Nếu không phải admin thì chỉ được update rating của chính mình
            if (!_currentUserService.IsAdmin && rating.UserId != _currentUserService.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} is not allowed to delete Rating {RatingId}",
                    _currentUserService.UserId,
                    request.Id);

                return TResult.Failure(
                    MessageKey.Forbidden,
                    ErrorCodes.FORBIDDEN);
            }

            ratingRepository.Remove(rating);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Deleted Product Rating {RatingId} Successfully",
                request.Id);

            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Delete Product Rating {RatingId} Failed",
                request.Id);

            return TResult.Failure(
                MessageKey.InternalError,
                ErrorCodes.SERVER_ERROR);
        }
    }
}
