namespace FlowerEcommerce.Domain.Entities;
public class Category : ModificationAuditedEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
