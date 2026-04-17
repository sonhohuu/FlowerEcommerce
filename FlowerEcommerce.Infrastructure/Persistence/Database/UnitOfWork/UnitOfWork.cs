using FlowerEcommerce.Application.Interfaces.Repositories;
using FlowerEcommerce.Domain.Entities.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Infrastructure.Persistence.Database.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDbContextTransaction? _transaction;

        // Cache repositories to avoid creating multiple instances for the same entity type
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Repository Access

        /// <summary>
        /// Get or create a repository for the specified entity type.
        /// Repositories are cached per entity type for the lifetime of this UnitOfWork.
        /// </summary>
        public IBaseRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var entityType = typeof(TEntity);

            if (_repositories.TryGetValue(entityType, out var existingRepo))
            {
                return (IBaseRepository<TEntity>)existingRepo;
            }

            var repository = new BaseRepository<TEntity>(_context, _httpContextAccessor);
            _repositories.TryAdd(entityType, repository);
            return repository;
        }

        #endregion

        #region Transaction Management

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null) return;
            _transaction =
                await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            _logger.LogInformation("Transaction started");
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                await (_transaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
                _logger.LogInformation("Transaction committed");
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await (_transaction?.RollbackAsync(cancellationToken) ?? Task.CompletedTask);
                _logger.LogInformation("Transaction rolled back");
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Execute an action within a transaction scope.
        /// Automatically commits on success or rolls back on exception.
        /// </summary>
        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            await BeginTransactionAsync(cancellationToken);
            try
            {
                await action();
                await CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// Execute a function within a transaction scope and return the result.
        /// Automatically commits on success or rolls back on exception.
        /// </summary>
        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default)
        {
            await BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await func();
                await CommitTransactionAsync(cancellationToken);
                return result;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        #endregion

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
