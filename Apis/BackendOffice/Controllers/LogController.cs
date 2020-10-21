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
        private readonly ILogQueries _logQueries;

        public LogController(
            ILogQueries logQueries,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _logQueries = logQueries;
        }

        [HttpGet("admin")]
        public async Task<PagedResponse<AdminLogModel>> GetAdminLogsAsync([FromQuery] AdminLogRequest request)
        {
            var logs = await _logQueries.GetAdminLogsAsync(request);
            
            return await logs.Select(q => new AdminLogModel
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
        public async Task<PagedResponse<ApiLogModel>> GetApiLogsAsync([FromQuery] ApiLogRequest request)
        {
            var logs = await _logQueries.GetApiLogsAsync(request);

            return await logs.Select(q => new ApiLogModel
            {
                    TrackingId = q.TrackingId,
                    Id = q.Id,
                    Ip = q.Ip,
                    Path = q.Path,
                    Method = q.Method,
                    Prefix = q.Prefix,
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
