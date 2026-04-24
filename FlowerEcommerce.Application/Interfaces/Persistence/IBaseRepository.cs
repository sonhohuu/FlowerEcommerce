namespace FlowerEcommerce.Application.Interfaces.UnitOfWork;

public interface IBaseRepository<T> where T : class, IBaseEntity
{
    #region ==================== UTILITIES ====================

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    #endregion

    #region ==================== CREATE ====================

    public void Add(T entity);

    public void AddRange(IEnumerable<T> entities);

    public void Attach(T entity);

    public void AttachRange(IEnumerable<T> entities);

    #endregion

    #region ==================== UPDATE ====================

    public void Update(T entity);

    public void UpdateRange(IEnumerable<T> entities);

    #endregion

    #region ==================== DELETE ====================

    public void Remove(T entity);

    public void RemoveRange(IEnumerable<T> entities);

    #endregion

    #region ==================== READ ====================

    public ValueTask<T?> FindByKeysAsync(
        CancellationToken cancellationToken = default,
        params object[] keyValues
    );

    public Task<bool> AllAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    public Task<bool> AnyAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );

    public Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );

    public Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    public Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    public Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    public Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
       Expression<Func<T, TResult>> selector,
       Expression<Func<T, bool>>? predicate = null,
       List<string>? includes = null,
       Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
       bool asNoTracking = false,
       CancellationToken cancellationToken = default
   );

    #endregion

    #region ==================== PAGINATION ====================

    public Task<(List<T>, int)> GetPaginationAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int page = 1,
        int pageSize = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    );

    public Task<(List<TResult>, int)> GetPaginationAsync<TResult>(
    Expression<Func<T, bool>>? predicate = null,
    List<string>? includes = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    int page = 1,
    int pageSize = AppConstants.DefaultPageSize,
    CancellationToken cancellationToken = default
);

    public Task<IPaginate<T>> GetPagingListAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<string>? includes = null,
        int page = 1,
        int size = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    );

    public Task<IPaginate<TResult>> GetPagingListAsync<TResult>(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<string>? includes = null,
        int page = 1,
        int size = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    );

    public Task<IPaginate<TResult>> GetPagingListAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<string>? includes = null,
        int page = 1,
        int size = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
  );

    public Task<(List<T>, int)> GetDtPaginationAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        string? orderStatement = null,
        int page = 0,
        int pageSize = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    );

    public Task<(List<TResult>, int)> GetDtPaginationAsync<TResult>(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        string? orderStatement = null,
        int page = 0,
        int pageSize = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    );

    #endregion
}
