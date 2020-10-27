using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Core.DataServices.Base.Extensions
{
    public static class SqlExtensions
    {
        public static async Task<IEnumerable<TEntity>> SqlQueryMultipleAsync<TEntity>(this IBaseRepository repository,
            string sql, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null,
            params SqlParameter[] parameters)
        {
            var conn = repository.GetDbConnection();

            if (parameters == null || parameters.Length == 0)
                return await conn.QueryAsync<TEntity>(sql, commandType: commandType, transaction: transaction);

            var param = GetDynamicParameters(parameters);

            return await conn.QueryAsync<TEntity>(sql, param, commandType: commandType, transaction: transaction);
        }

        public static async Task<(IEnumerable<TEntity>, int)> SqlQueryByPagingAsync<TEntity>(
            this IBaseRepository repository, string sql, CommandType commandType = CommandType.StoredProcedure,
            IDbTransaction transaction = null, params SqlParameter[] parameters)
        {
            var conn = repository.GetDbConnection();

            if (parameters == null || parameters.Length == 0)
                throw new ArgumentException("No Parameters: Paging query has no paging parameters.");

            var param = GetDynamicParameters(parameters);

            var multi = await conn.QueryMultipleAsync(sql, param, commandType: commandType, transaction: transaction);

            return (await multi.ReadAsync<TEntity>(), await multi.ReadFirstOrDefaultAsync<int>());
        }

        public static async Task<TEntity> SqlQuerySingleAsync<TEntity>(this IBaseRepository repository, string sql,
            CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null,
            params SqlParameter[] parameters)
        {
            var conn = repository.GetDbConnection();

            if (parameters == null || parameters.Length == 0)
                return await conn.QueryFirstOrDefaultAsync<TEntity>(sql, commandType: commandType,
                    transaction: transaction);

            var param = GetDynamicParameters(parameters);

            return await conn.QueryFirstOrDefaultAsync<TEntity>(sql, param, commandType: commandType,
                transaction: transaction);
        }

        public static async Task<int> ExecuteSqlCommandAsync(this IBaseRepository repository, string sql,
            CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null,
            params SqlParameter[] parameters)
        {
            var conn = repository.GetDbConnection();

            if (parameters == null || parameters.Length == 0)
                return await conn.ExecuteAsync(sql, commandType: commandType, transaction: transaction);

            var param = GetDynamicParameters(parameters);

            return await conn.ExecuteAsync(sql, param, commandType: commandType, transaction: transaction);
        }

        public static int ExecuteSqlCommand(this IBaseRepository repository, string sql,
            CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null,
            params SqlParameter[] parameters)
        {
            var conn = repository.GetDbConnection();

            if (parameters == null || parameters.Length == 0)
                return conn.Execute(sql, commandType: commandType, transaction: transaction);

            var param = GetDynamicParameters(parameters);

            return conn.Execute(sql, param, commandType: commandType, transaction: transaction);
        }

        private static DynamicParameters GetDynamicParameters(IEnumerable<SqlParameter> parameters)
        {
            var param = new DynamicParameters();

            foreach (var p in parameters)
            {
                if (p.SqlDbType != SqlDbType.Structured)
                {
                    param.Add(p.ParameterName, p.Value, p.DbType);
                    continue;
                }

                if (p.Value is not DataTable)
                    throw new ArgumentException(
                        $"Parameter({p.ParameterName}): DataTable is expected for Structured SqlDbType but is {p.Value.GetType()} ");

                param.Add(p.ParameterName, ((DataTable) p.Value).AsTableValuedParameter());
            }

            return param;
        }
    }
}