using FlowerEcommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowerEcommerce.Infrastructure.Persistence.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        #region DbSet

        public DbSet<ApplicationUser> AppUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductRating> ProductRatings { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }

        #endregion
    }
}
