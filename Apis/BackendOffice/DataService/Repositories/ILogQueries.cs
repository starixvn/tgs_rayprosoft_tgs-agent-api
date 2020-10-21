using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Apis.BackendOffice.Controllers.Messages.Log;
using TGS.Core.DataServices.Log.Models;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public interface ILogQueries
    {
        IQueryable<T> GetQueries<T>() where T : class;

        Task<T> GetSingleAsync<T>(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> preQuery = null) where T : class;

        Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<IEnumerable<AdminLog>> GetAdminLogsAsync(AdminLogRequest request);
        Task<IEnumerable<ApiLog>> GetApiLogsAsync(ApiLogRequest request);
    }
}