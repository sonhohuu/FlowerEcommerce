using FlowerEcommerce.View.Interfaces;
using FlowerEcommerce.View.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FlowerEcommerce.View.Services;

public class ProductApiService : IProductApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IMemoryCache _cache;

    // Key prefix để tránh collision với cache khác
    private const string SlugCachePrefix = "slug2id_";
    private static readonly TimeSpan SlugCacheTtl = TimeSpan.FromMinutes(30);

    public ProductApiService(HttpClient httpClient, IHttpContextAccessor contextAccessor, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _contextAccessor = contextAccessor;
        _cache = cache;
    }

    public async Task<(List<ProductViewModel> Items, int TotalPages, int CurrentPage)> GetProductsAsync(
        int page = 1,
        int pageSize = 12,
        ulong? categoryId = null,
        string? search = null,
        string? sort = null)
    {
        var query = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString(),
        };

        if (!string.IsNullOrEmpty(search))
            query["searchKeyword"] = search;

        // Pass categoryId directly to the API (server-side filter)
        if (categoryId.HasValue)
            query["categoryId"] = categoryId.Value.ToString();

        var queryString = string.Join("&", query
            .Where(x => x.Value != null)
            .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<PaginatedData<ProductListApiDto>>>(
                $"api/product?{queryString}");

        if (response?.Success != true || response.Data is null)
            return (new List<ProductViewModel>(), 0, page);

        var items = response.Data.Items.Select(MapToViewModel).ToList();

        // Client-side sort (replace with server param when API supports it)
        items = sort switch
        {
            "price-asc" => items.OrderBy(p => p.Price).ToList(),
            "price-desc" => items.OrderByDescending(p => p.Price).ToList(),
            _ => items,
        };

        return (items, response.Data.TotalPages, response.Data.Page);
    }

    // ── GET DETAIL BY SLUG (public, dùng ở Page) ─────────────────────────
    public async Task<ProductDetailViewModel?> GetProductDetailBySlugAsync(string slug)
    {
        var id = await ResolveSlugToIdAsync(slug);
        if (id is null) return null;

        return await GetProductDetailByIdAsync(id.Value);
    }

    // ── RESOLVE SLUG → ID ────────────────────────────────────────────────
    private async Task<ulong?> ResolveSlugToIdAsync(string slug)
    {
        var cacheKey = SlugCachePrefix + slug;

        // 1. Cache hit → trả về ngay
        if (_cache.TryGetValue(cacheKey, out ulong cachedId))
            return cachedId;

        // 2. Cache miss → gọi API list, tìm slug khớp chính xác
        //    Dùng searchKeyword để thu hẹp tập kết quả
        try
        {
            var response = await _httpClient
                .GetFromJsonAsync<ApiResponse<PaginatedData<ProductListApiDto>>>(
                    $"api/product?page=1&pageSize=50&searchKeyword={Uri.EscapeDataString(slug)}");

            if (response?.Success == true && response.Data?.Items is not null)
            {
                // Cache tất cả item trả về, tiết kiệm round-trip sau này
                CacheSlugMappings(response.Data.Items);

                var match = response.Data.Items.FirstOrDefault(x => x.Slug == slug);
                if (match is not null) return match.Id;
            }
        }
        catch { /* log nếu cần */ }

        return null;
    }

    // ── GET DETAIL BY ID (internal) ───────────────────────────────────────
    private async Task<ProductDetailViewModel?> GetProductDetailByIdAsync(ulong id)
    {
        try
        {
            var response = await _httpClient
                .GetFromJsonAsync<ApiResponse<ProductDetailApiDto>>(
                    $"api/product/{id}");

            if (response?.Success != true || response.Data is null)
                return null;

            return MapToDetailViewModel(response.Data);
        }
        catch { return null; }
    }

    public async Task<ReviewPaginatedResult> GetProductRatingsAsync(ulong productId, int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient
                .GetFromJsonAsync<ApiResponse<PaginatedData<ReviewViewModel>>>(
                    $"api/productrating?ProductId={productId}&page={page}&pageSize={pageSize}");

            if (response?.Success != true || response.Data is null)
                return new ReviewPaginatedResult();

            return new ReviewPaginatedResult
            {
                Items = response.Data.Items.Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    Score = r.Score,
                    Comment = r.Comment,
                    UserName = r.UserName ?? "Khách hàng"
                }).ToList(),
                TotalPages = response.Data.TotalPages,
                CurrentPage = response.Data.Page,
                TotalCount = response.Data.Items.Count   // tuỳ cấu trúc PaginatedData
            };
        }
        catch { return new ReviewPaginatedResult(); }
    }

    public async Task<(bool Success, string? Message, int StatusCode)> CreateRatingAsync(CreateReviewRequest request)
    {
        try
        {
            // Lấy access_token từ cookie của request hiện tại
            var token = _contextAccessor.HttpContext?.Request.Cookies["access_token"];

            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var res = await _httpClient.PostAsJsonAsync("api/productrating", request);

            return ((int)res.StatusCode) switch
            {
                200 or 201 => (true, null, 200),
                401 => (false, "UNAUTHORIZED", 401),
                _ => (false, await res.Content.ReadAsStringAsync(), (int)res.StatusCode)
            };
        }
        catch (Exception ex)
        {
            return (false, ex.Message, 500);
        }
    }

    // ── CACHE HELPER ─────────────────────────────────────────────────────
    private void CacheSlugMappings(IEnumerable<ProductListApiDto> items)
    {
        foreach (var dto in items)
        {
            if (string.IsNullOrEmpty(dto.Slug)) continue;

            _cache.Set(
                SlugCachePrefix + dto.Slug,
                dto.Id,
                SlugCacheTtl);
        }
    }

    private static ProductViewModel MapToViewModel(ProductListApiDto dto)
    {
        int? discountPercent = null;
        if (dto.OriginalPrice.HasValue && dto.OriginalPrice > 0
            && dto.Price.HasValue && dto.Price > 0)
        {
            discountPercent = (int)Math.Round(
                (1 - dto.Price.Value / dto.OriginalPrice.Value) * 100);
        }

        return new ProductViewModel
        {
            Id = (int)dto.Id,
            Name = dto.Name,
            Price = dto.Price ?? 0,
            Slug = dto.Slug,
            OriginalPrice = dto.OriginalPrice,
            DiscountPercent = discountPercent,
            IsContactPrice = dto.IsContactPrice,
            ImageUrl = dto.MainImage?.SecureUrl ?? "/images/placeholder.jpg",
        };
    }

    private static ProductDetailViewModel MapToDetailViewModel(ProductDetailApiDto dto)
    {
        var images = dto.FileAttachments
            .OrderByDescending(f => f.IsMain)
            .ThenBy(f => f.SortOrder)
            .Select(f => f.SecureUrl)
            .ToList();

        return new ProductDetailViewModel
        {
            Id = (int)dto.Id,
            Name = dto.Name,
            Sku = dto.ProductDetail?.Sku ?? "",
            Images = images.Any() ? images : new() { "/images/no-image.png" },
            Price = dto.Price ?? 0,
            OriginalPrice = dto.OriginalPrice,
            IsContactPrice = dto.IsContactPrice,
            IsOutOfStock = dto.IsOutOfStock,
            Category = dto.Category?.Name ?? "",
            CategorySlug = dto.Category?.Slug ?? "",
            Description = dto.Description,
            SizePrices = dto.SizePrices
                .Select(s => new SizePrice { Label = s.Label, Price = s.Price })
                .ToList(),
        };
    }
}
