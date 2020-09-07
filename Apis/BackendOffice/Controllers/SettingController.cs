using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Settings;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Apis.BackendOffice.Services;
using TGS.Infrastructures.Json;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : BackendOfficeControllerBase
    {
        private readonly ISettingQueries _settingQueries;
        private readonly ISettingCommands _settingCommands;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly Dictionary<string, Func<string, bool>> _validateSettingMap;
        private readonly string[] _requiredSettings = { "Menus" };
        public SettingController(
            ISettingQueries settingQueries,
            ISettingCommands settingCommands,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _settingQueries = settingQueries;
            _settingCommands = settingCommands;
            _jsonSerializationProvider = jsonSerializationProvider;
            _validateSettingMap = new Dictionary<string, Func<string, bool>>
            {
                { "Menus", ValidateValueByKey<MenuSetting[]> }
            };
        }

        [Route("")]
        [HttpGet, Description("Get setting list")]
        public Task<PagedResponse<SettingModel>> GetSettingsAsync([FromQuery] GetSettingsRequest request)
        {
            return _settingQueries.GetSettingQueries()
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
        public async Task<GetSettingResponse> GetSettingByKeyAsync(string key)
        {
            var setting = await _settingQueries.GetSettingByKeyAsync(key);
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

        [HttpPut("{key}"), Description("Update setting by key")]
        public async Task<MessageResponse> UpdateSettingAsync(string key, [FromBody] UpdateSettingRequest request)
        {
            var setting = await _settingQueries.GetSettingByKeyAsync(key);
            if (setting == null)
            {
                return CreateInvalidResponse<MessageResponse>($"{key} {Resources.Common.NotFound}");
            }

            if (_validateSettingMap[key] != null)
            {
                if (_validateSettingMap[key](request.Value) == false)
                {
                    return CreateInvalidResponse<MessageResponse>(Resources.Common.InvalidFormat);
                }
            }

            setting.Value = request.Value;
            setting.Description = request.Description;
            setting.UpdatedOn = DateTimeOffset.Now;
            setting.UpdatedByUsername = Username;

            await _settingCommands.UpdateSettingAsync(setting);
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

            var setting = await _settingQueries.GetSettingByKeyAsync(key);
            if (setting == null)
            {
                return CreateInvalidResponse<MessageResponse>($"{key} {Resources.Common.NotFound}");
            }

            await _settingCommands.DeleteSettingAsync(setting);
            return CreateValidResponse<MessageResponse>();
        }


        private bool ValidateValueByKey<T>(string value)
        {
            try
            {
                _jsonSerializationProvider.DeserializeFromString<T>(value);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
