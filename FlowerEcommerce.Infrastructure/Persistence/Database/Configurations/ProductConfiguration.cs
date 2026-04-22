namespace FlowerEcommerce.Infrastructure.Persistence.Database.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasOne(e => e.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(e => e.Ratings)
           .WithOne(r => r.Product)
           .HasForeignKey(r => r.ProductId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
