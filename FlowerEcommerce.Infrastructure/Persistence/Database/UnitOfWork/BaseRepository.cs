namespace FlowerEcommerce.Infrastructure.Persistence.Database.UnitOfWork;
public class BaseRepository<T> : IBaseRepository<T> where T : class, IBaseEntity
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(ApplicationDbContext context)
    {
        _dbContext = context;
        _dbSet = context.Set<T>();
    }

    #region ==================== CREATE ====================

    public virtual void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public virtual void Attach(T entity)
    {
        _dbSet.Attach(entity);
    }

    public virtual void AttachRange(IEnumerable<T> entities)
    {
        _dbSet.AttachRange(entities);
    }

    #endregion

    #region ==================== UPDATE ====================

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    #endregion

    #region ==================== DELETE ====================

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    #endregion

    #region ==================== READ ====================

    protected virtual IQueryable<T> AsNoTracking()
    {
        return _dbSet.AsNoTracking();
    }

    protected virtual IQueryable<T> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }

    // For composite keys
    public virtual async ValueTask<T?> FindByKeysAsync(
        CancellationToken cancellationToken = default,
        params object[] keyValues
    )
    {
        return await _dbSet.FindAsync(keyValues, cancellationToken);
    }

    public virtual async Task<bool> AllAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await AsNoTracking().AllAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        return predicate is null
            ? await AsNoTracking().AnyAsync(cancellationToken)
            : await AsNoTracking().AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        return predicate is null
            ? await AsNoTracking().CountAsync(cancellationToken)
            : await AsNoTracking().CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, asNoTracking);

        return await dbSetQueryable.ToListAsync(cancellationToken);
    }

    public virtual async Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, asNoTracking);

        return await dbSetQueryable
            .AsSplitQuery()
            .ProjectToType<TResult>()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, asNoTracking);

        return await dbSetQueryable
            .AsSplitQuery()
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, asNoTracking);

        return await dbSetQueryable.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, asNoTracking);

        return await dbSetQueryable
            .AsSplitQuery()
            .ProjectToType<TResult>()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> FirstOrDefaultAsync<TResult>(
       Expression<Func<T, TResult>> selector,
       Expression<Func<T, bool>>? predicate = null,
       List<string>? includes = null,
       Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
       bool asNoTracking = false,
       CancellationToken cancellationToken = default
   )
    {
        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, asNoTracking);

        return await dbSetQueryable
            .AsSplitQuery()
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);
    }

    #endregion

    #region ==================== PAGINATION ====================

    public Task<IPaginate<T>> GetPagingListAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<string>? includes = null,
        int page = 1,
        int size = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, true);
        page = page < 1 ? 1 : page;
        size = size is < 1 or > AppConstants.MaxPageSize ? AppConstants.DefaultPageSize : size;
        return dbSetQueryable.ToPaginateAsync(page, size, 1, cancellationToken);
    }

    public Task<IPaginate<TResult>> GetPagingListAsync<TResult>(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<string>? includes = null,
        int page = 1,
        int size = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, true);
        page = page < 1 ? 1 : page;
        size = size is < 1 or > AppConstants.MaxPageSize ? AppConstants.DefaultPageSize : size;
        return dbSetQueryable.ProjectToType<TResult>().ToPaginateAsync(page, size, 1, cancellationToken);
    }

    public Task<IPaginate<TResult>> GetPagingListAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<string>? includes = null,
        int page = 1,
        int size = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, true);
        page = page < 1 ? 1 : page;
        size = size is < 1 or > AppConstants.MaxPageSize ? AppConstants.DefaultPageSize : size;
        return dbSetQueryable.Select(selector).ToPaginateAsync(page, size, 1, cancellationToken);
    }

    public virtual async Task<(List<T>, int)> GetPaginationAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int page = 1,
        int pageSize = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > AppConstants.MaxPageSize ? AppConstants.DefaultPageSize : pageSize;

        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, true);

        var totalRecords = await dbSetQueryable.CountAsync(cancellationToken);

        var items = await dbSetQueryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalRecords);
    }

    public virtual async Task<(List<TResult>, int)> GetPaginationAsync<TResult>(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int page = 1,
        int pageSize = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > AppConstants.MaxPageSize ? AppConstants.DefaultPageSize : pageSize;

        var dbSetQueryable = BuildQuery(predicate, includes, orderBy, true);

        var totalRecords = await dbSetQueryable.CountAsync(cancellationToken);

        var items = await dbSetQueryable
            .AsSplitQuery()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<TResult>()
            .ToListAsync(cancellationToken);

        return (items, totalRecords);
    }

    public virtual async Task<(List<T>, int)> GetDtPaginationAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        string? orderStatement = null,
        int page = 0,
        int pageSize = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        page = page < 0 ? 0 : page;
        pageSize = pageSize is < 1 or > AppConstants.MaxPageSize ? AppConstants.DefaultPageSize : pageSize;

        var dbSetQueryable = BuildQuery(predicate, includes, CustomOrderBy, true);

        var totalRecords = await dbSetQueryable.CountAsync(cancellationToken);

        var items = await dbSetQueryable
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalRecords);

        IOrderedQueryable<T> CustomOrderBy(IQueryable<T> q)
        {
            //if (!string.IsNullOrWhiteSpace(orderStatement)) return q.OrderBy(orderStatement);

            return typeof(ICreationAuditedEntity).IsAssignableFrom(typeof(T))
                ? q.OrderByDescending(x => EF.Property<DateTime>(x, nameof(ICreationAuditedEntity.CreatedAt)))
                : q.OrderByDescending(x => x.Id);
        }
    }


    public virtual async Task<(List<TResult>, int)> GetDtPaginationAsync<TResult>(
        Expression<Func<T, bool>>? predicate = null,
        List<string>? includes = null,
        string? orderStatement = null,
        int page = 0,
        int pageSize = AppConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        page = page < 0 ? 0 : page;
        pageSize = pageSize is < 1 or > AppConstants.MaxPageSize ? AppConstants.DefaultPageSize : pageSize;

        var dbSetQueryable = BuildQuery(predicate, includes, CustomOrderBy, true);

        var totalRecords = await dbSetQueryable.CountAsync(cancellationToken);

        var items = await dbSetQueryable
            .AsSplitQuery()
            .Skip(page * pageSize)
            .Take(pageSize)
            .ProjectToType<TResult>()
            .ToListAsync(cancellationToken);

        return (items, totalRecords);

        IOrderedQueryable<T> CustomOrderBy(IQueryable<T> q)
        {
            //if (!string.IsNullOrWhiteSpace(orderStatement)) return q.OrderBy(orderStatement);

            return typeof(ICreationAuditedEntity).IsAssignableFrom(typeof(T))
                ? q.OrderByDescending(x => EF.Property<DateTime>(x, nameof(ICreationAuditedEntity.CreatedAt)))
                : q.OrderByDescending(x => x.Id);
        }
    }

    #endregion

    #region ==================== UTILITIES ====================

    protected virtual Func<IQueryable<T>, IQueryable<T>>? GetInclude(List<string> includePaths)
    {
        if (includePaths.Count == 0) return null;

        return q =>
        {
            // Làm sạch + khử trùng chính tả (giữ Ordinal để bắt typo)
            var paths = includePaths
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToList();

            var query = paths.Aggregate(q, (cur, path) => cur.Include(path));

            var rootCount = paths
                .Select(p =>
                {
                    var dot = p.IndexOf('.');
                    return dot >= 0 ? p[..dot] : p;
                })
                .Distinct(StringComparer.Ordinal)
                .Count();

            // Rule: nếu có >= 2 root Includes thì tách query để tránh cartesian explosion
            if (rootCount >= 2) query = query.AsSplitQuery();

            return query;
        };
    }

    protected virtual IQueryable<T> BuildQuery(
        Expression<Func<T, bool>>? predicate,
        List<string>? includes,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy,
        bool asNoTracking = false
    )
    {
        var dbSetQueryable = asNoTracking ? AsNoTracking() : AsQueryable();

        var includeFunc = GetInclude(includes ?? []);
        if (includeFunc != null) dbSetQueryable = includeFunc(dbSetQueryable);

        if (predicate != null) dbSetQueryable = dbSetQueryable.Where(predicate);

        return orderBy != null ? orderBy(dbSetQueryable) : dbSetQueryable.OrderByDescending(x => x.Id);
    }

    protected ApplicationDbContext GetDbContext()
    {
        if (_dbContext is ApplicationDbContext dbContext) return dbContext;

        throw new InvalidOperationException($"DbContext is not a {nameof(ApplicationDbContext)}.");
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
