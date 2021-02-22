using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Core.DataServices.Base.Extensions
{
    public static class SqlExtensions
    {
        public static Task<IEnumerable<TEntity>> SqlQueryMultipleAsync<TEntity>(this IBaseRepository repository,
            string sql, CommandType commandType = CommandType.StoredProcedure,
            IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var conn = repository.GetDbConnection();
            var transaction = repository.CurrentTransaction?.GetDbTransaction();
            return conn.QueryAsync<TEntity>(sql, parameters, commandType: commandType, transaction: transaction);
        }

        public static async Task<(IEnumerable<TEntity>, int)> SqlQueryByPagingAsync<TEntity>(
            this IBaseRepository repository, string sql, CommandType commandType = CommandType.StoredProcedure,
            IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var conn = repository.GetDbConnection();
            var transaction = repository.CurrentTransaction?.GetDbTransaction();

            var multi = await conn.QueryMultipleAsync(sql, parameters, commandType: commandType, transaction: transaction);
            return (await multi.ReadAsync<TEntity>(), await multi.ReadFirstOrDefaultAsync<int>());
        }

        public static Task<TEntity> SqlQuerySingleAsync<TEntity>(this IBaseRepository repository, string sql,
            CommandType commandType = CommandType.StoredProcedure, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var conn = repository.GetDbConnection();
            var transaction = repository.CurrentTransaction?.GetDbTransaction();

            return conn.QueryFirstOrDefaultAsync<TEntity>(sql, parameters, commandType: commandType,
                transaction: transaction);
        }

        public static Task<int> ExecuteSqlCommandAsync(this IBaseRepository repository, string sql,
            CommandType commandType = CommandType.StoredProcedure, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var conn = repository.GetDbConnection();
            var transaction = repository.CurrentTransaction?.GetDbTransaction();

            return conn.ExecuteAsync(sql, parameters, commandType: commandType, transaction: transaction);
        }
    }
}