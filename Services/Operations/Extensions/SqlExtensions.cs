using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TGS.Core.Models.Interfaces;

namespace TGS.Services.Operations.Extensions
{
    public static class SqlExtensions
    {
        public static async Task<IEnumerable<TEntity>> SqlQueryListAsync<TEntity>(this IBaseRepository repository, string sql, SqlParameter[] parameters)
        {
            using var conn = repository.GetDbConnection();
 
            if (parameters == null || parameters.Length == 0)
            {
                return await conn.QueryAsync<TEntity>(sql);
            }

            var param = new DynamicParameters();
                
            foreach(var p in parameters)
            {
                param.Add(p.ParameterName, p.Value, p.DbType);
            }

            return await conn.QueryAsync<TEntity>(sql, param);
        }

        public static async Task<TEntity> SqlQueryAsync<TEntity>(this IBaseRepository repository, string sql, SqlParameter[] parameters)
        {
            using var conn = repository.GetDbConnection();
 
            if (parameters == null || parameters.Length == 0)
            {
                return await conn.QueryFirstAsync<TEntity>(sql);
            }

            var param = new DynamicParameters();
                
            foreach(var p in parameters)
            {
                param.Add(p.ParameterName, p.Value, p.DbType);
            }

            return await conn.QueryFirstAsync<TEntity>(sql, param);
        }

        public static async Task<int> ExecuteSqlCommandAsync(this IBaseRepository repository, string sql, SqlParameter[] parameters)
        {
            using var conn = repository.GetDbConnection();

            if (parameters == null || parameters.Length == 0)
            {
                return await conn.ExecuteAsync(sql);
            }

            var param = new DynamicParameters();
                
            foreach(var p in parameters)
            {
                param.Add(p.ParameterName, p.Value, p.DbType);
            }

            return await conn.ExecuteAsync(sql, param);
        }
    }
}
