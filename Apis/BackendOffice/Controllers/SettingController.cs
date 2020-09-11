using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Settings;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Apis.BackendOffice.Extensions;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : BackendOfficeControllerBase
    {
        private readonly IGenericQueries<Setting> _queries;
        private readonly IGenericCommands<Setting> _commands;
        private readonly string[] _requiredSettings = { "Menus" };
        public SettingController(
            IGenericQueries<Setting> queries,
            IGenericCommands<Setting> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [Route("")]
        [HttpGet, Description("Get setting list")]
        public Task<PagedResponse<SettingModel>> GetSettingsAsync([FromQuery] GetSettingsRequest request)
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
        public async Task<GetSettingResponse> GetSettingAsync(string key)
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

        [HttpPost(""), Description("Create setting by key")]
        public async Task<MessageResponse> CreateSettingAsync([FromBody] CreateSettingRequest request)
        {
            var settingExisted = await _queries.GetSingleAsync(p => p.Key == request.Key);
            if (settingExisted != null)
            {
                return CreateInvalidResponse<MessageResponse>($"{request.Key} {Resources.Setting.KeyExists}");
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
        public async Task<MessageResponse> UpdateSettingAsync(string key, [FromBody] UpdateSettingRequest request)
        {
            var setting = await _queries.GetSingleAsync(p => p.Key == key);
            if (setting == null)
            {
                return CreateInvalidResponse<MessageResponse>($"{key} {Resources.Common.NotFound}");
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
        public async Task<MessageResponse> DeleteSettingAsync(string key)
        {
            if (_requiredSettings.Contains(key))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Common.RequiredSetting);
            }

            var setting = await _queries.GetSingleAsync(p => p.Key == key);
            if (setting == null)
            {
                return CreateInvalidResponse<MessageResponse>($"{key} {Resources.Common.NotFound}");
            }

            await _commands.DeleteAsync(setting);
            return CreateValidResponse<MessageResponse>();
        }
    }
}
