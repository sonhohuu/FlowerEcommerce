using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Domain.Entities.Base
{
    public interface ICreationAuditedEntity : IBaseEntity
    {
        public ulong? CreatorId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreationAuditedEntity : BaseEntity, ICreationAuditedEntity
    {
        public ulong? CreatorId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
