using FlowerEcommerce.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Domain.Entities
{
    public class Order : ModificationAuditedEntity
    {
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public ApplicationUser? User { get; set; }
        public IList<OrderItem> Items { get; set; } = [];
    }
}
