using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace TGS.Core.DataServices.Base.Interfaces
{
    public interface IBaseRepository : IAsyncDisposable
    {
        IDbContextTransaction CurrentTransaction { get; }

        ChangeTracker ChangeTracker { get; }

        Task<bool> CanConnectionAsync();

        void SetTimeoutToSeconds(int seconds);

        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level);

        IDbConnection GetDbConnection();

        Task BulkInsertAsync<T>(IEnumerable<T> entities) where T : class;

        Task BulkUpdateAsync<T>(IEnumerable<T> entities) where T : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task ExecuteMigrationAndSeedingAsync(IWebHostEnvironment env);

        DbSet<T> Set<T>() where T : class;

        EntityEntry<T> Entry<T>(T entity) where T : class;
    }
}
