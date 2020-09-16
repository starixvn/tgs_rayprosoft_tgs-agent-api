using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace TGS.Core.DataServices.Base.Interfaces
{
    public interface IBaseRepository : IAsyncDisposable
    {
        Task<bool> CanConnectionAsync();

        void SetTimeoutToSeconds(int seconds);

        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level);

        IDbConnection GetDbConnection();

        void BulkInsert<T>(IList<T> entities, BulkConfig bulkConfig = null, Action<decimal> progress = null) where T : class;

        Task SaveChangesAsync();

        void SaveChanges();
    }
}
