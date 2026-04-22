namespace FlowerEcommerce.Infrastructure.Persistence.Database.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasOne(e => e.Order)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
