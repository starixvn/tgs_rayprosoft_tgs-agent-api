using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.BackendOffice.Controllers.Messages.Log;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Log.Models;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : BaseContextController
    {
        private readonly ILogQueries<AdminLog> _adminLogQueries;
        private readonly ILogQueries<ApiLog> _apiLogQueries;

        public LogController(
            ILogQueries<AdminLog> logQueries,
            ILogQueries<ApiLog> apiLogQueries,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _adminLogQueries = logQueries;
            _apiLogQueries = apiLogQueries;
        }

        [HttpGet("admin")]
        public Task<PagedResponse<AdminLogModel>> GetAdminLogs([FromQuery] AdminLogRequest request)
        {
            return _adminLogQueries.GetQueries()
                .Where(request.From, q => request.From >= q.RecordedOn)
                .Where(request.To, q => request.From <= q.RecordedOn)
                .Where(request.TrackingId, q => request.TrackingId == q.TrackingId)
                .Select(q => new AdminLogModel
                {
                    TrackingId = q.TrackingId,
                    Id = q.Id,
                    Ip = q.Ip,
                    Path = q.Path,
                    Method = q.Method,
                    Description = q.Description,
                    Exception = q.Exception,
                    StackTrace = q.StackTrace,
                    ServerName = q.ServerName,
                    RequestHeaders = q.RequestHeaders,
                    RequestContent = q.RequestContent,
                    ResponseContent = q.ResponseContent,
                    ResponseHeaders = q.ResponseHeaders
                })
                .CreatePagedResponseAsync(request);
        }

        [HttpGet("api")]
        public Task<PagedResponse<ApiLogModel>> GetAPILogs([FromQuery] ApiLogRequest request)
        {
            return _apiLogQueries.GetQueries()
                .Where(request.From, q => request.From >= q.RecordedOn)
                .Where(request.To, q => request.From <= q.RecordedOn)
                .Where(request.TrackingId, q => request.TrackingId == q.TrackingId)
                .Select(q => new ApiLogModel
                {
                    TrackingId = q.TrackingId,
                    Id = q.Id,
                    Ip = q.Ip,
                    Path = q.Path,
                    Method = q.Method,
                    ServerName = q.ServerName,
                    RequestHeaders = q.RequestHeaders,
                    RequestContent = q.RequestContent,
                    ResponseContent = q.ResponseContent,
                    ResponseHeaders = q.ResponseHeaders
                })
                .CreatePagedResponseAsync(request);
        }
    }
}
