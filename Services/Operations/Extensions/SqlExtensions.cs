using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Services.Operations.Extensions
{
    public static class SqlExtensions
    {
        public static async Task<IEnumerable<TEntity>> SqlQueryListAsync<TEntity>(this IBaseRepository repository, string sql, CommandType commandType = CommandType.StoredProcedure, params SqlParameter[] parameters)
        {
            using var conn = repository.GetDbConnection();
 
            if (parameters == null || parameters.Length == 0)
            {
                return await conn.QueryAsync<TEntity>(sql, commandType: commandType);
            }

            var param = new DynamicParameters();
                
            foreach(var p in parameters)
            {
                param.Add(p.ParameterName, p.Value, p.DbType);
            }

            return await conn.QueryAsync<TEntity>(sql, param, commandType: commandType);
        }

        public static async Task<TEntity> SqlQueryAsync<TEntity>(this IBaseRepository repository, string sql, CommandType commandType = CommandType.StoredProcedure, params SqlParameter[] parameters)
        {
            using var conn = repository.GetDbConnection();
 
            if (parameters == null || parameters.Length == 0)
            {
                return await conn.QueryFirstOrDefaultAsync<TEntity>(sql, commandType: commandType);
            }

            var param = new DynamicParameters();
                
            foreach(var p in parameters)
            {
                param.Add(p.ParameterName, p.Value, p.DbType);
            }

            return await conn.QueryFirstOrDefaultAsync<TEntity>(sql, param, commandType: commandType);
        }

        public static async Task<int> ExecuteSqlCommandAsync(this IBaseRepository repository, string sql, CommandType commandType = CommandType.StoredProcedure, params SqlParameter[] parameters)
        {
            using var conn = repository.GetDbConnection();

            if (parameters == null || parameters.Length == 0)
            {
                return await conn.ExecuteAsync(sql, commandType: commandType);
            }

            var param = new DynamicParameters();
                
            foreach(var p in parameters)
            {
                param.Add(p.ParameterName, p.Value, p.DbType);
            }

            return await conn.ExecuteAsync(sql, param, commandType: commandType);
        }
    }
}
