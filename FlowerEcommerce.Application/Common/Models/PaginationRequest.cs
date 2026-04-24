using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.Application.Common.Models;

public class PaginationRequest
{
    [Range(1, int.MaxValue)] public int Page { get; set; } = 1;
    [Range(1, AppConstants.MaxPageSize)]
    public int PageSize { get; set; } = AppConstants.DefaultPageSize;
}
