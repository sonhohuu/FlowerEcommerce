using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Domain.Entities
{
    public class ProductDetail : DeletionAuditedEntity
    {
        public string Sku { get; set; } = "";
        public string Slug { get; set; } = "";
        public IList<ProductSizePrices> SizePrices { get; set; } = [];
    }
}
