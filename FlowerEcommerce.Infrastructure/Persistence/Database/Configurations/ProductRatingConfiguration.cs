namespace FlowerEcommerce.Infrastructure.Persistence.Database.Configurations;

public class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
{
    public void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        builder.HasOne(e => e.Product)
            .WithMany(e => e.Ratings)
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
