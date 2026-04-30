namespace FlowerEcommerce.Domain.Entities;

public class Product : DeletionAuditedEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal? Price { get; set; } = 0;
    public decimal? OriginalPrice { get; set; }
    public bool IsContactPrice { get; set; } = false;
    public bool Status { get; set; } = true;
    public bool IsOutOfStock { get; set; } = false;

    #region Category

    public ulong? CategoryId { get; set; }
    public Category? Category { get; set; }

    #endregion

    #region ProductDetail

    public ulong? ProductDetailId { get; set; }
    public ProductDetail? ProductDetail { get; set; }

    #endregion

    #region FileAttachment

    public IList<FileAttachment> FileAttachments { get; set; } = [];

    #endregion

    #region Rating

    public IList<ProductRating> Ratings { get; set; } = [];

    #endregion
}
