namespace FlowerEcommerce.Domain.Entities.Base;

public interface IModificationAuditedEntity : ICreationAuditedEntity
{
    public ulong? LastModifierId { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

public class ModificationAuditedEntity : CreationAuditedEntity, IModificationAuditedEntity
{
    public ulong? LastModifierId { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
