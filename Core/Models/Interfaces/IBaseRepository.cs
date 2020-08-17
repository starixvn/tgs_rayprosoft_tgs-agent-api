using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace TGS.Core.Models.Interfaces
{
    public interface IBaseRepository : IAsyncDisposable
    {
        Task<bool> CanConnectionAsync();

        void SetTimeoutToSeconds(int seconds);

        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level);

        DbConnection GetDbConnection();
    }
}
