using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Settings;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : BaseContextController
    {
        private readonly IBackendOfficeQueries<Setting> _queries;
        private readonly IBackendOfficeCommands<Setting> _commands;

        public SettingController(
            IBackendOfficeQueries<Setting> queries,
            IBackendOfficeCommands<Setting> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [Route("")]
        [HttpGet, Description("Get setting list")]
        public Task<PagedResponse<SettingModel>> GetListAsync([FromQuery] GetSettingsRequest request)
        {
            return _queries.GetQueries()
                .Where(request.Key, p => p.Key.Contains(request.Key))
                .Select(setting => new SettingModel
                {
                    Key = setting.Key,
                    Value = setting.Value,
                    Description = setting.Description,
                    CreatedByUsername = setting.CreatedByUsername,
                    UpdatedByUsername = setting.UpdatedByUsername,
                    CreatedOn = setting.CreatedOn,
                    UpdatedOn = setting.UpdatedOn
                }).CreatePagedResponseAsync(request);
        }

        [Route("{key}")]
        [HttpGet, Description("Get setting by key")]
        public async Task<GetSettingResponse> GetSingleAsync(string key)
        {
            var setting = await _queries.GetSingleAsync(p => p.Key == key);
            if (setting == null)
            {
                return CreateInvalidResponse<GetSettingResponse>(Resources.Common.NotFound);
            }

            return new GetSettingResponse
            {
                Valid = true,
                Setting = new SettingModel
                {
                    Key = setting.Key,
                    Value = setting.Value,
                    Description = setting.Description,
                    CreatedByUsername = setting.CreatedByUsername,
                    UpdatedByUsername = setting.UpdatedByUsername,
                    CreatedOn = setting.CreatedOn,
                    UpdatedOn = setting.UpdatedOn
                }
            };
        }

        [HttpPost(""), Description("Create a setting")]
        public async Task<MessageResponse> CreateAsync([FromBody] CreateSettingRequest request)
        {
            var settingExisted = await _queries.GetSingleAsync(p => p.Key == request.Key);
            if (settingExisted != null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Setting.KeyExists);
            }

            var setting = new Setting
            {
                Key = request.Key,
                Value = request.Value,
                Description = request.Description,
                UpdatedOn = DateTimeOffset.Now,
                UpdatedByUsername = Username
            };

            await _commands.CreateAsync(setting);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{key}"), Description("Update setting by key")]
        public async Task<MessageResponse> UpdateAsync(string key, [FromBody] UpdateSettingRequest request)
        {
            var setting = await _queries.GetSingleAsync(p => p.Key == key);
            if (setting == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Common.NotFound);
            }

            setting.Value = request.Value;
            setting.Description = request.Description;
            setting.UpdatedOn = DateTimeOffset.Now;
            setting.UpdatedByUsername = Username;

            await _commands.UpdateAsync(setting);
            return CreateValidResponse<MessageResponse>();
        }

        [Route("{key}")]
        [HttpDelete, Description("Delete setting by key")]
        public async Task<MessageResponse> DeleteAsync(string key)
        {
            var setting = await _queries.GetSingleAsync(p => p.Key == key);
            if (setting == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Common.NotFound);
            }

            await _commands.DeleteAsync(setting);
            return CreateValidResponse<MessageResponse>();
        }
    }
}
