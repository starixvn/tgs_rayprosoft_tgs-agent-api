using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace TGS.Core.DataServices.Base.Interfaces
{
    public interface IBaseRepository : IAsyncDisposable
    {
        Task<bool> CanConnectionAsync();

        void SetTimeoutToSeconds(int seconds);

        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level);

        IDbConnection GetDbConnection();

        Task SaveChangesAsync();
    }
}
