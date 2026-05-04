namespace FlowerEcommerce.Application.Handlers.Users.Queries;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, TResult<IPaginate<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<TResult<IPaginate<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetPagingListAsync<UserDto>(
            predicate: u => (string.IsNullOrEmpty(request.SearchKeyword) || u.UserName.Contains(request.SearchKeyword) || u.Email.Contains(request.SearchKeyword)),
            selector: u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role.ToString(),
                Status = u.Status.ToString(),

                OrderCount = u.Orders.Count(),
                TotalSpent = u.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0
            },
            page: request.Page,
            size: request.PageSize
        );

        return TResult<IPaginate<UserDto>>.Success(users);
    }
}
