namespace FlowerEcommerce.View.Models;

public class ProductListApiDto
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool IsContactPrice { get; set; }
    public bool IsOutOfStock { get; set; }
    public bool Status { get; set; }
    public string? Slug { get; set; }
    public MainImageDto? MainImage { get; set; }
}

public class MainImageDto
{
    public string? SecureUrl { get; set; }
    public string? Url { get; set; }
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public string? AltText { get; set; }
}
