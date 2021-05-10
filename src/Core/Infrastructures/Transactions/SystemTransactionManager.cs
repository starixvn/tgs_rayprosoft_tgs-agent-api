using System;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;

namespace TGS.Core.Infrastructures.Transactions
{
    public sealed class SystemTransactionManager
    {
        private readonly ITgsLog _log;

        public SystemTransactionManager(ITgsLog log)
        {
            _log = log.SetLogger("System Transaction Manager");
        }

        public async Task<TResult> ExecuteTransactionAsync<TContext, TResult>(
            TContext context,
            IsolationLevel level,
            string externalTransactionId,
            Func<TContext, Task<TResult>> execute,
            Func<TContext, TResult, Task> onPostCommit = null,
            Func<TContext, TResult, Exception, Task> onPostRollback = null,
            bool autoDetectChangesEnabled = true) where TContext : IBaseRepository
        {
            await using var transaction = await context.BeginTransactionAsync(level);
            ExceptionDispatchInfo errorInfo;
            TResult result = default;

            if (!autoDetectChangesEnabled)
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;
            }

            try
            {
                result = await execute(context);

                await transaction.CommitAsync();

                try
                {
                    // commit succeeded, log any post error. otherwise, transaction rollback will fail.
                    if (onPostCommit != null)
                    {
                        await onPostCommit(context, result);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError("Error during post transaction committed.", ex);
                }

                return result;
            }
            catch (Exception ex)
            {
                errorInfo = ExceptionDispatchInfo.Capture(ex);
            }

            // log the error in saving the commit
            _log.LogWarning(
                $"Error during transaction with External Id:'{externalTransactionId}' {errorInfo.SourceException.GetMessageChain()}",
                errorInfo.SourceException);

            try
            {
                if (transaction.GetDbTransaction().Connection != null)
                {
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex2)
            {
                _log.LogWarning(
                    $"Error during rolling back transaction with External Id:'{externalTransactionId}' {errorInfo.SourceException.GetMessageChain()}",
                    ex2);
            }

            if (onPostRollback != null)
            {
                await onPostRollback(context, result, errorInfo.SourceException);
            }

            if (!autoDetectChangesEnabled)
            {
                context.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            errorInfo.Throw();

            throw new InvalidOperationException("This part of the code should never be reached");
        }
    }
}