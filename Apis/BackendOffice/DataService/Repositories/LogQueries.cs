using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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

        IQueryable<T> ILogQueries.GetQueries<T>() => _repository.Set<T>().AsQueryable();

        Task<bool> ILogQueries.ExistsAsync<T>(Expression<Func<T, bool>> predicate) =>
            _repository.Set<T>().AnyAsync(predicate);

        Task<T> ILogQueries.GetSingleAsync<T>(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> preQuery)
        {
            preQuery ??= c => c;
            return preQuery(_repository.Set<T>()).FirstOrDefaultAsync(predicate);
        }

        async Task<(IEnumerable<AdminLogModel>, int)> ILogQueries.GetAdminLogsAsync(AdminLogRequest request)
        {
            const string spName = "dbo.log_get_admin_logs";
            var results = await _repository.SqlQueryMultipleAsync<AdminLogModel>(spName,
                parameters: new Dictionary<string, object>
                {
                    {"_from", request.From},
                    {"_to", request.To},
                    {"_tracking_id", request.TrackingId},
                    {"_ip", request.Ip},
                    {"_path", request.Path},
                    {"_method", request.Method},
                    {"_page_size", request.ShowAll ? int.MaxValue : request.Rows},
                    {"_page_number", request.ShowAll ? 1 : request.Page},
                    {"_sort_by", request.SortBy},
                    {"_is_desc", request.IsDesc}
                });
            return (results, results.Any() ? results.First().TotalCount : 0);
        }

        async Task<(IEnumerable<ApiLogModel>, int)> ILogQueries.GetApiLogsAsync(ApiLogRequest request)
        {
            const string spName = "dbo.log_get_api_logs";
            var results = await _repository.SqlQueryMultipleAsync<ApiLogModel>(spName,
                parameters: new Dictionary<string, object>
                {
                    {"_from", request.From},
                    {"_to", request.To},
                    {"_tracking_id", request.TrackingId},
                    {"_ip", request.Ip},
                    {"_path", request.Path},
                    {"_method", request.Method},
                    {"_page_size", request.ShowAll ? int.MaxValue : request.Rows},
                    {"_page_number", request.ShowAll ? 1 : request.Page},
                    {"_sort_by", request.SortBy},
                    {"_is_desc", request.IsDesc}
                });
            return (results, results.Any() ? results.First().TotalCount : 0);
        }

        Task<LogDetailModel> ILogQueries.GetAdminLogAsync(Guid trackingId)
        {
            const string spName = "dbo.log_get_admin_log";
            return _repository.SqlQuerySingleAsync<LogDetailModel>(spName, parameters: new Dictionary<string, object>
            {
                {"_tracking_id", trackingId}
            });
        }

        Task<LogDetailModel> ILogQueries.GetApiLogAsync(Guid trackingId)
        {
            const string spName = "dbo.log_get_api_log";
            return _repository.SqlQuerySingleAsync<LogDetailModel>(spName, parameters: new Dictionary<string, object>
            {
                {"_tracking_id", trackingId}
            });
        }
    }
}