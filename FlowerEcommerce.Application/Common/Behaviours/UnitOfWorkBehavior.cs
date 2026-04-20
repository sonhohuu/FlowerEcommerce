using FlowerEcommerce.Application.Common.Attributes;
using FlowerEcommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Application.Common.Behaviours
{
    public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;

        public UnitOfWorkBehavior(IUnitOfWork unitOfWork, ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!request.GetType().GetCustomAttributes(typeof(EnableUnitOfWorkAttribute), true).Any())
            {
                return await next();
            }

            var requestName = typeof(TRequest).Name;
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                var response = await next();
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Transaction committed for {RequestName}", requestName);
                return response;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict detected for {RequestName}. Rolling back transaction...", requestName);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                // For concurrency conflicts, we need to return a proper response instead of throwing
                // This requires the response type to have a failure case
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition().Name.Contains("TResult"))
                {
                    // Try to create a failure response using reflection
                    var failureMethod = typeof(TResponse).GetMethod("Failure", new[] { typeof(string), typeof(string) });
                    if (failureMethod != null)
                    {
                        var failureResponse = failureMethod.Invoke(null, new object[] {
                        "Dữ liệu đã được thay đổi bởi người dùng khác. Vui lòng tải lại trang và thử lại.",
                        "CONCURRENT_UPDATE"
                    });
                        return (TResponse)failureResponse!;
                    }
                }

                // If we can't create a proper failure response, re-throw
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction failed for {RequestName}. Rolling back...", requestName);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
