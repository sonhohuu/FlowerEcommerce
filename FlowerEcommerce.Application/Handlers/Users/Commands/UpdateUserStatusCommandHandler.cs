namespace FlowerEcommerce.Application.Handlers.Users.Commands;

public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, TResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserStatusCommandHandler> _logger;

    public UpdateUserStatusCommandHandler(IUserRepository userRepository, ILogger<UpdateUserStatusCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<TResult> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.FirstOrDefaultAsync(
                predicate: u => u.Id == request.UserId,
                cancellationToken: cancellationToken);
            if (user == null)
            {
                return TResult.Failure(MessageKey.UserNotFound);
            }
            user.Status = request.Status;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated User {UserId} status to {Status} successfully", request.UserId, request.Status);
            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status for User {UserId}", request.UserId);
            return TResult.Failure("An error occurred while updating the user status.");
        }
    }
}
