using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Log;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Log;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public class LogQueries : ILogQueries
    {
        private readonly ILogRepository _repository;

        public LogQueries(ILogRepository repository)
        {
            _repository = repository;
        }

        IQueryable<T> ILogQueries.GetQueries<T>()
        {
            return _repository.Set<T>().AsQueryable();
        }

        Task<bool> ILogQueries.ExistsAsync<T>(Expression<Func<T, bool>> predicate)
        {
            return _repository.Set<T>().AnyAsync(predicate);
        }

        Task<T> ILogQueries.GetSingleAsync<T>(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> preQuery)
        {
            preQuery ??= c => c;
            return preQuery(_repository.Set<T>()).FirstOrDefaultAsync(predicate);
        }

        Task<(IEnumerable<AdminLogModel>, int)> ILogQueries.GetAdminLogsAsync(AdminLogRequest request)
        {
            const string spName = "usp_AdminLogs_GetLogs";
            return _repository.SqlQueryByPagingAsync<AdminLogModel>(spName, parameters: new[]
            {
                new SqlParameter
                {
                    ParameterName = "@from",
                    Value = request.From,
                    DbType = DbType.DateTimeOffset
                },
                new SqlParameter
                {
                    ParameterName = "@to",
                    Value = request.To,
                    DbType = DbType.DateTimeOffset
                },
                new SqlParameter
                {
                    ParameterName = "@trackingId",
                    Value = request.TrackingId,
                    DbType = DbType.String
                },
                new SqlParameter
                {
                    ParameterName = "@ip",
                    Value = request.Ip,
                    DbType = DbType.String
                },
                new SqlParameter
                {
                    ParameterName = "@path",
                    Value = request.Path,
                    DbType = DbType.String
                },
                new SqlParameter
                {
                    ParameterName = "@method",
                    Value = request.Method,
                    DbType = DbType.String
                },
                new SqlParameter
                {
                    ParameterName = "@pageSize",
                    Value = request.ShowAll ? int.MaxValue : request.Rows,
                    DbType = DbType.Int32
                },
                new SqlParameter
                {
                    ParameterName = "@pageNumber",
                    Value = request.ShowAll ? 1 : request.Page,
                    DbType = DbType.Int32
                },
                new SqlParameter
                {
                    ParameterName = "@sortBy",
                    Value = request.SortBy,
                    DbType = DbType.String
                },
                new SqlParameter
                {
                    ParameterName = "@isDesc",
                    Value = request.IsDesc,
                    DbType = DbType.Boolean
                }
            });
        }

        Task<(IEnumerable<ApiLogModel>, int)> ILogQueries.GetApiLogsAsync(ApiLogRequest request)
        {
            const string spName = "usp_ApiLogs_GetLogs";
            return _repository.SqlQueryByPagingAsync<ApiLogModel>(spName, parameters: new[]
            {
                new SqlParameter
                {
                    ParameterName = "@from",
                    Value = request.From,
                    DbType = DbType.DateTimeOffset
                },
                new SqlParameter
                {
                    ParameterName = "@to",
                    Value = request.To,
                    DbType = DbType.DateTimeOffset
                },
                new SqlParameter
                {
                    ParameterName = "@trackingId",
                    Value = request.TrackingId,
                    DbType = DbType.String
                },
                new SqlParameter
                {
                    ParameterName = "@ip",
                    Value = request.Ip,
                    DbType = DbType.String
                },
                new SqlParameter
                {
                    ParameterName = "@path",
                    Value = request.Path,
                    DbType = DbType.String
                },
                new SqlParameter
                {
                    ParameterName = "@method",
                    Value = request.Method,
                    DbType = DbType.String
                }
            });
        }
    }
}