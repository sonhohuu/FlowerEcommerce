namespace FlowerEcommerce.Infrastructure.Persistence.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    private bool _ignoreSoftDeleteFilter;
    private readonly ILogger<ApplicationDbContext> _logger;

    public DatabaseFacade DbInstance => Database;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        const int maxRetries = 5; // tránh lặp vô hạn khi bị spam cập nhật
        var attempt = 0;

        while (true)
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex) when (attempt++ < maxRetries)
            {
                _logger.LogWarning(
                    ex, "Concurrency conflict on {saveChangesAsyncName} (attempt {attempt}/{maxRetries}).",
                    nameof(SaveChangesAsync), attempt, maxRetries
                );

                // --- Strategy: DATABASE WINS ---
                // Nạp lại giá trị mới nhất từ DB cho mọi entry bị conflict rồi thử save lại.
                foreach (var entry in ex.Entries)
                    // Nếu entity đã bị xóa ngoài DB, ReloadAsync sẽ ném lỗi → để vòng catch ngoài xử lý
                    await entry.ReloadAsync(cancellationToken);
                // --- Strategy: CLIENT WINS (alternative) ---
                // Giữ nguyên giá trị hiện tại của client; chỉ cập nhật OriginalValues
                // để EF xem thay đổi của client là mới nhất.
                // foreach (var entry in ex.Entries)
                // {
                //     var dbValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                //     if (dbValues != null)
                //     {
                //         entry.OriginalValues.SetValues(dbValues);
                //     }
                // }
                // (Chọn một trong hai chiến lược; đừng dùng cả hai)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SaveChangesAsync)} failed.");
                throw;
            }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Don't move below line to anywhere, this must be placed at the beginning
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // --- LOGIC MỚI FIX LỖI CACHING ---
        // Lấy MethodInfo của hàm ConfigureSoftDeleteFilter ở trên
        var methodInfo = typeof(ApplicationDbContext)
            .GetMethod(nameof(ConfigureSoftDeleteFilter), BindingFlags.Instance | BindingFlags.Public);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Chỉ áp dụng với entity có Soft Delete
            if (!typeof(IDeletionAuditedEntity).IsAssignableFrom(entityType.ClrType)) continue;

            // Gọi hàm ConfigureSoftDeleteFilter<T> thông qua Reflection
            var genericMethod = methodInfo?.MakeGenericMethod(entityType.ClrType);
            genericMethod?.Invoke(this, [modelBuilder]);
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<Enum>().HaveConversion<string>().HaveMaxLength(50);
    }

    public void ConfigureSoftDeleteFilter<TEntity>(ModelBuilder builder)
    where TEntity : class, IDeletionAuditedEntity
    {
        // Ở đây, "this" được compiler hiểu là Context instance runtime
        builder.Entity<TEntity>().HasQueryFilter(e =>
            e.DeletedAt == null || _ignoreSoftDeleteFilter);
    }

    public IDisposable DisableSoftDelete()
    {
        return new SoftDeleteScope(this);
    }

    private class SoftDeleteScope : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly bool _originalState;

        public SoftDeleteScope(ApplicationDbContext context)
        {
            _context = context;
            _originalState = _context._ignoreSoftDeleteFilter;

            _context._ignoreSoftDeleteFilter = true;
        }

        public void Dispose()
        {
            _context._ignoreSoftDeleteFilter = _originalState;
        }
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
