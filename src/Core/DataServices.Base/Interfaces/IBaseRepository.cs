using System;
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
        ChangeTracker ChangeTracker { get; }

        IDbContextTransaction CurrentTransaction { get; }

        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level);

        Task<bool> CanConnectionAsync();

        EntityEntry<T> Entry<T>(T entity) where T : class;

        IDbConnection GetDbConnection();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        DbSet<T> Set<T>() where T : class;

        void SetTimeoutToSeconds(int seconds);
    }
}
