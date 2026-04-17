using FlowerEcommerce.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Domain.Entities
{
    public class OrderItem : ModificationAuditedEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
