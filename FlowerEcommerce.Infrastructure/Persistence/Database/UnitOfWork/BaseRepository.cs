using FlowerEcommerce.Application.Common.Extensions;
using FlowerEcommerce.Application.Interfaces.Repositories;
using FlowerEcommerce.Domain.Entities.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FlowerEcommerce.Infrastructure.Persistence.Database.UnitOfWork
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ulong? _currentUserId;

        public BaseRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _currentUserId = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value is { } id
                ? ulong.TryParse(id, out var userId) ? userId : null
                : null;
        }

        #region Query Methods

        public virtual async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>>[]? filters = null,
            string? orderBy = null,
            int skip = 0,
            int limit = 0,
            bool isNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = BuildQuery(filters, orderBy, skip, limit, isNoTracking, includes);
            return await query.ToListAsync();
        }

        public virtual async Task<List<TDto>> FindAsync<TDto>(
            Expression<Func<TEntity, bool>>[]? filters = null,
            string? orderBy = null,
            int skip = 0,
            int limit = 0,
            bool isNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var entities = await FindAsync(filters, orderBy, skip, limit, isNoTracking, includes);
            return entities.ProjectTo<TEntity, TDto>();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>[]? filters = null, bool isNoTracking = false)
        {
            var query = isNoTracking ? QueryNoTracking() : Query();
            query = ApplyFilters(query, filters);
            return await query.CountAsync();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>[]? filters = null, bool isNoTracking = false)
        {
            var query = isNoTracking ? QueryNoTracking() : Query();
            query = ApplyFilters(query, filters);
            return await query.AnyAsync();
        }

        public virtual async Task<bool> ExistsAsync(ulong id)
        {
            return await _dbSet.AnyAsync(x => x.Id == id);
        }

        public virtual async Task<bool> ExistsAsync(IEnumerable<ulong> ids)
        {
            var idList = ids.ToList();
            var count = await _dbSet.CountAsync(x => idList.Contains(x.Id));
            return count == idList.Count;
        }

        #endregion

        #region FindOne Methods

        public virtual async Task<TEntity?> FindByIdAsync(ulong id, bool isNoTracking = false, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = isNoTracking ? QueryNoTracking() : Query();
            query = ApplyIncludes(query, includes);
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task<TDto?> FindByIdAsync<TDto>(ulong id, bool isNoTracking = false, params Expression<Func<TEntity, object>>[] includes)
        {
            var entity = await FindByIdAsync(id, isNoTracking, includes);
            return entity == null ? default : entity.ProjectTo<TEntity, TDto>();
        }

        public virtual async Task<TEntity?> FindOneAsync(
            Expression<Func<TEntity, bool>>[]? filters = null,
            string? orderBy = null,
            bool isNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = isNoTracking ? QueryNoTracking() : Query();
            query = ApplyFilters(query, filters);
            query = ApplyIncludes(query, includes);
            query = ApplyOrderBy(query, orderBy);
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<TDto?> FindOneAsync<TDto>(
            Expression<Func<TEntity, bool>>[]? filters = null,
            string? orderBy = null,
            bool isNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var entity = await FindOneAsync(filters, orderBy, isNoTracking, includes);
            return entity == null ? default : entity.ProjectTo<TEntity, TDto>();
        }

        #endregion

        #region CUD Operations

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual async Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            return Task.CompletedTask;
        }

        public virtual async Task<bool> DeleteAsync(ulong id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity == null) return false;

            return await DeleteAsync(entity, cancellationToken);
        }

        public virtual Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(true);
        }

        public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Query Builders

        public virtual IQueryable<TEntity> Query()
        {
            return _dbSet.AsQueryable();
        }

        public virtual IQueryable<TEntity> QueryNoTracking()
        {
            return Query().AsNoTracking();
        }

        #endregion

        #region Protected Helpers

        protected IQueryable<TEntity> BuildQuery(
            Expression<Func<TEntity, bool>>[]? filters,
            string? orderBy,
            int skip,
            int limit,
            bool isNoTracking,
            Expression<Func<TEntity, object>>[]? includes)
        {
            var query = isNoTracking ? QueryNoTracking() : Query();
            query = ApplyFilters(query, filters);
            query = ApplyIncludes(query, includes);
            query = ApplyOrderBy(query, orderBy);
            query = ApplyPaging(query, skip, limit);
            return query;
        }

        protected static IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query, Expression<Func<TEntity, bool>>[]? filters)
        {
            if (filters == null || filters.Length == 0) return query;
            return filters.Aggregate(query, (current, filter) => current.Where(filter));
        }

        protected static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, Expression<Func<TEntity, object>>[]? includes)
        {
            if (includes == null || includes.Length == 0) return query;
            return includes.Aggregate(query, (current, include) => current.Include(include));
        }

        protected static IQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query, string? orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy)) return query;

            var parts = orderBy.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var propertyName = parts[0];
            var isDescending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            // Convert first letter to uppercase for property matching
            propertyName = char.ToUpper(propertyName[0]) + propertyName[1..];

            return isDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, propertyName))
                : query.OrderBy(e => EF.Property<object>(e, propertyName));
        }

        protected static IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, int skip, int limit)
        {
            if (skip > 0) query = query.Skip(skip);
            if (limit > 0) query = query.Take(limit);
            return query;
        }

        //protected static IQueryable<TEntity> ApplyDateFilters<TQuery>(IQueryable<TEntity> query, TQuery queryDto) where TQuery : BaseQueryDto
        //{
        //    if (queryDto.CreatedAt != null && queryDto.CreatedAt != default)
        //    {
        //        var dateQuery = queryDto.CreatedAt.DatetimeQuery();
        //        if (dateQuery != null)
        //        {
        //            query = query.Where(x => x.CreatedAt >= dateQuery.StartDateAt && x.CreatedAt <= dateQuery.EndDateAt);
        //        }
        //    }

        //    if (queryDto.CreateAtFrom != null && queryDto.CreateAtFrom != default)
        //    {
        //        var dateQuery = queryDto.CreateAtFrom.DatetimeQuery();
        //        if (dateQuery != null)
        //        {
        //            query = query.Where(x => x.CreatedAt >= dateQuery.StartDateAt);
        //        }
        //    }

        //    if (queryDto.CreateAtTo != null && queryDto.CreateAtTo != default)
        //    {
        //        var dateQuery = queryDto.CreateAtTo.DatetimeQuery();
        //        if (dateQuery != null)
        //        {
        //            query = query.Where(x => x.CreatedAt <= dateQuery.EndDateAt);
        //        }
        //    }

        //    return query;
        //}

        #endregion
    }
}
