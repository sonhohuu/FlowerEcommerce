using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Domain.Entities
{
    public class ProductSizePrices : ModificationAuditedEntity
    {
        public string Label { get; set; } = "";
        public decimal Price { get; set; }
        public ulong? ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
    }
}
