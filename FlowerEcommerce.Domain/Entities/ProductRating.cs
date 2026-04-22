namespace FlowerEcommerce.Domain.Entities
{
    public class ProductRating : ModificationAuditedEntity
    {
        public int Score { get; set; } // 1-5
        public string Comment { get; set; } = string.Empty;

        #region Product

        public ulong ProductId { get; set; }
        public Product? Product { get; set; }

        #endregion
        
        #region User

        public ApplicationUser? User { get; set; }
        public ulong? UserId { get; set; }

        #endregion
    }
}
