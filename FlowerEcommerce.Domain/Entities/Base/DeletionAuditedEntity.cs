using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Domain.Entities.Base
{
    public interface IDeletionAuditedEntity : IModificationAuditedEntity
    {
        public bool IsDeleted { get; }
        public ulong? DeleterId { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class DeletionAuditedEntity : ModificationAuditedEntity, IDeletionAuditedEntity
    {
        public bool IsDeleted => DeletedAt != null;

        public ulong? DeleterId { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
