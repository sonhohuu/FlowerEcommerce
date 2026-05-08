namespace FlowerEcommerce.View.Models;

public class ReviewViewModel
{
    public ulong Id { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public string? UserName { get; set; }
}

public class ReviewPaginatedResult
{
    public List<ReviewViewModel> Items { get; set; } = new();
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int TotalCount { get; set; }
}

public class CreateReviewRequest
{
    public ulong ProductId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}
