using System;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Services.Operations
{
    public interface ITransactionManager
    {
        Task<TResult> ExecuteTransactionAsync<TContext, TResult>(
            Func<TContext> contextFactory,
            IsolationLevel level,
            string externalTransactionId,
            Func<TContext, IDbContextTransaction, Task<TResult>> execute,
            Func<TContext, TResult, Task> onPostCommit = null,
            Func<TContext, TResult, Exception, Task> onPostRollback = null)
            where TContext : IBaseRepository;
    }

    public sealed class TransactionManager : ITransactionManager
    {
        //private readonly IUgsLog _log;

        // public TransactionManager(IUgsLog log)
        // {
        //     _log = log.SetLogger("Transaction Manager");
        // }

        async Task<TResult> ITransactionManager.ExecuteTransactionAsync<TContext, TResult>(
            Func<TContext> contextFactory,
            IsolationLevel level,
            string externalTransactionId,
            Func<TContext, IDbContextTransaction, Task<TResult>> execute,
            Func<TContext, TResult, Task> onPostCommit,
            Func<TContext, TResult, Exception, Task> onPostRollback)
        {
            await using var context = contextFactory();
            await using var transaction = await context.BeginTransactionAsync(level);
            ExceptionDispatchInfo errorInfo;
            TResult result = default;

            try
            {
                result = await execute(context, transaction);

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
                    //_log.LogError("Error during post transaction committed.", ex);
                }

                return result;
            }
            catch (Exception ex)
            {
                errorInfo = ExceptionDispatchInfo.Capture(ex);
            }

            // log the error in saving the commit
            //_log.LogWarning($"Error during transaction with External Id:'{externalTransactionId}' {errorInfo.SourceException.GetMessageChain()}",
            //    errorInfo.SourceException);

            try
            {
                if (transaction.GetDbTransaction().Connection != null)
                {
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex2)
            {
                //_log.LogWarning(
                //    $"Error during rolling back transaction with External Id:'{externalTransactionId}' {errorInfo.SourceException.GetMessageChain()}",
                //    ex2);
            }

            if (onPostRollback != null)
            {
                await onPostRollback(context, result, errorInfo.SourceException);
            }

            errorInfo.Throw();

            throw new InvalidOperationException("This part of the code should never be reached");
        }
    }
}
