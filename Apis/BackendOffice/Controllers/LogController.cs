using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Log;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices.Base.Extensions;
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
            var (logs, totalCount) = await _logQueries.GetAdminLogsAsync(request);

            return await logs.CreatePagedResponseAsync(totalCount);
        }

        [HttpGet("admin/{id:Guid}")]
        public async Task<LogDetailResponse> GetAdminLogAsync(Guid id)
        {
            var logDetail = await _logQueries.GetAdminLogAsync(id);

            return new LogDetailResponse
            {
                Valid = true,
                LogDetail = logDetail
            };
        }

        [HttpGet("api")]
        public async Task<PagedResponse<ApiLogModel>> GetApiLogsAsync([FromQuery] ApiLogRequest request)
        {
            var (logs, totalCount) = await _logQueries.GetApiLogsAsync(request);

            return await logs.CreatePagedResponseAsync(totalCount);
        }

        [HttpGet("api/{id:Guid}")]
        public async Task<LogDetailResponse> GetApiLogAsync(Guid id)
        {
            var logDetail = await _logQueries.GetApiLogAsync(id);

            return new LogDetailResponse
            {
                Valid = true,
                LogDetail = logDetail
            };
        }
    }
}