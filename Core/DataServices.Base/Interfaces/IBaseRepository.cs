using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace TGS.Core.DataServices.Base.Interfaces
{
    public interface IBaseRepository : IAsyncDisposable
    {
        IDbContextTransaction CurrentTransaction { get; }

        Task<bool> CanConnectionAsync();

        void SetTimeoutToSeconds(int seconds);

        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level);

        IDbConnection GetDbConnection();

        Task BulkInsertAsync<T>(IList<T> entities, BulkConfig bulkConfig = null, Action<decimal> progress = null) where T : class;

        Task BulkInsertOrUpdateAsync<T>(IList<T> entities, BulkConfig bulkConfig = null, Action<decimal> progress = null) where T : class;

        Task SaveChangesAsync();

        Task ExecuteMigrationAndSeedingAsync(bool addSeeding = false);

        DbSet<T> Set<T>() where T : class;

        EntityEntry<T> Entry<T>(T entity) where T : class;
    }
}
