namespace FlowerEcommerce.Domain.Entities;

public class Product : DeletionAuditedEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal? Price { get; set; } = 0;

    #region Category

    public ulong? CategoryId { get; set; }
    public Category? Category { get; set; }

    #endregion

    #region FileAttachment

    public IList<FileAttachment> FileAttachments { get; set; } = [];

    #endregion

    #region Rating

    public IList<ProductRating> Ratings { get; set; } = [];

    #endregion
}
