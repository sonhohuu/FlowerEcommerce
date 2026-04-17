using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Domain.Entities
{
    public class ProductRating
    {
        public int Score { get; set; } // 1-5
        public string Comment { get; set; } = string.Empty;

        #region Product

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        #endregion
        
        #region User

        public ApplicationUser? User { get; set; }

        #endregion
    }
}
