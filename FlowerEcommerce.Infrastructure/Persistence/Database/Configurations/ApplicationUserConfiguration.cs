namespace FlowerEcommerce.Infrastructure.Persistence.Database.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(e => e.Creator)
            .WithMany()
            .HasForeignKey(e => e.CreatorId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(e => e.LastModifier)
            .WithMany()
            .HasForeignKey(e => e.LastModifierId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
