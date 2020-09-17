using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices.Models;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : BaseUserContextController
    {
        private readonly ITgsQueries<Player> _queries;
        private readonly ITgsCommands<Player> _commands;

        public PlayerController(
            ITgsQueries<Player> queries,
            ITgsCommands<Player> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        //[Route("")]
        //[HttpGet, Description("Get setting list")]
        //public Task<PagedResponse<SettingModel>> GetListAsync([FromQuery] GetSettingsRequest request)
        //{
        //    return _queries.GetQueries()
        //        .Where(request.Key, p => p.Key.Contains(request.Key))
        //        .Select(setting => new SettingModel
        //        {
        //            Key = setting.Key,
        //            Value = setting.Value,
        //            Description = setting.Description,
        //            CreatedByUsername = setting.CreatedByUsername,
        //            UpdatedByUsername = setting.UpdatedByUsername,
        //            CreatedOn = setting.CreatedOn,
        //            UpdatedOn = setting.UpdatedOn
        //        }).CreatePagedResponseAsync(request);
        //}
    }
}
