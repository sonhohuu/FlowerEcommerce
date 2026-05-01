namespace FlowerEcommerce.View.Models;

public class ProductDetailApiDto
{
    public ulong Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal? Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool IsContactPrice { get; set; }
    public bool IsOutOfStock { get; set; }
    public bool Status { get; set; }
    public ProductDetailInfoDto? ProductDetail { get; set; }
    public List<SizePriceApiDto> SizePrices { get; set; } = new();
    public List<FileAttachmentDto> FileAttachments { get; set; } = new();
    public CategoryDto? Category { get; set; }
}

public class ProductDetailInfoDto
{
    public string Sku { get; set; } = "";
    public string Slug { get; set; } = "";
}

public class SizePriceApiDto
{
    public string Label { get; set; } = "";
    public decimal Price { get; set; }
}

public class FileAttachmentDto
{
    public string SecureUrl { get; set; } = "";
    public string Url { get; set; } = "";
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public string? AltText { get; set; }
}

public class CategoryDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
}
