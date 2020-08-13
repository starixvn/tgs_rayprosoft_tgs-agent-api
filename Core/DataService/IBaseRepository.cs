using System;
using System.Threading.Tasks;

namespace TGS.Core.DataServices
{
    public interface IBaseRepository : IDisposable
    {
        Action<string> Log { get; set; }

        int Version { get; }

        bool HasChanges { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync();

        Task<int> ExecuteSqlCommandAsync(string sql, params object[] args);

        int ExecuteSqlCommand(string sql, params object[] args);

        Task<TEntity[]> SqlQueryArrayAsync<TEntity>(string sql, params object[] parameters);

        Task<TEntity> SqlQueryAsync<TEntity>(string sql, params object[] parameters);
    }
}
