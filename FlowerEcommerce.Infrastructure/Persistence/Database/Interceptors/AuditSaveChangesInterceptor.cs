namespace FlowerEcommerce.Infrastructure.Persistence.Database.Interceptors;

public class AuditSaveChangesInterceptor(
    ICurrentUserService currentUserService,
    IDateTimeService dateTimeService
) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        UpdateEntities(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var currentUserId = currentUserService.UserId;
        var currentDateTime = dateTimeService.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // ReSharper disable once MergeIntoPattern
            if (entry.State == EntityState.Deleted && entry.Entity is IDeletionAuditedEntity deleteEntity)
            {
                // 1. Đổi trạng thái từ Deleted -> Modified (để không xóa thật trong DB)
                entry.State = EntityState.Modified;

                // 2. Cập nhật thông tin người xóa
                deleteEntity.DeleterId = currentUserId;
                deleteEntity.DeletedAt = currentDateTime;
            }

            if (
                entry.State == EntityState.Added &&
                (
                    // ReSharper disable once MergeIntoLogicalPattern
                    entry.Entity is ICreationAuditedEntity ||
                    entry.Entity is IModificationAuditedEntity
                )
            )
            {
                var c = (ICreationAuditedEntity)entry.Entity;
                c.CreatorId = currentUserId;
                c.CreatedAt = currentDateTime;
            }

            // ReSharper disable once InvertIf
            if (
                entry.Entity is IModificationAuditedEntity m &&
                (
                    entry.State == EntityState.Added ||
                    entry.State == EntityState.Modified ||
                    entry.HasChangedOwnedEntities()
                )
            )
            {
                m.LastModifierId = currentUserId;
                m.LastModifiedAt = currentDateTime;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified
        );
    }
}
