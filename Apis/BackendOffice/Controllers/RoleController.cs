using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Roles;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Apis.BackendOffice.Services;
using TGS.Infrastructures.Json;

namespace TGS.Apis.BackendOffice.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public partial class RoleController : BackendOfficeControllerBase
    {
        private readonly IRoleQueries _roleQueries;
        private readonly ISettingQueries _settingQueries;
        private readonly IRoleCommands _roleCommands;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public RoleController(
            IRoleQueries roleQueries,
            ISettingQueries settingQueries,
            IRoleCommands roleCommands,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _roleQueries = roleQueries;
            _roleCommands = roleCommands;
            _settingQueries = settingQueries;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        [Route("")]
        [HttpGet, Description("Get roles")]
        public Task<PagedResponse<RoleModel>> GetRolesAsync([FromQuery] GetRolesRequest request)
        {
            return _roleQueries.GetRoleQueries()
                .Where(request.Name, p => p.Name.Contains(request.Name))
                .Select(role => new RoleModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    DisplayName = role.DisplayName,
                    Description = role.Description,
                    CreatedOn = role.CreatedOn,
                    UpdatedOn = role.UpdatedOn,
                    CreatedByUsername = role.CreatedByUsername,
                    UpdatedByUsername = role.CreatedByUsername

                })
                .CreatePagedResponseAsync(request);
        }


        [Route("{roleId}")]
        [HttpGet, Description("Get role")]
        public async Task<GetRoleResponse> GetRoleAsync(Guid roleId)
        {
            var role = await _roleQueries.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return CreateInvalidResponse<GetRoleResponse>($"{Resources.Common.NotFound}");
            }

            return new GetRoleResponse
            {
                Valid = true,
                MenuSettings = await GetMenuSettingAsync(role.MenuSetting)
            };
        }

        private async Task<MenuSetting[]> GetMenuSettingAsync(string menuSetting)
        {
            var setting = await _settingQueries.GetSettingByKeyAsync("Menus");
            var menuSettingTemplate = _jsonSerializationProvider.DeserializeFromString<MenuSetting[]>(setting.Value);
            var roleMenus = _jsonSerializationProvider.DeserializeFromString<MenuSetting[]>(menuSetting);

            foreach (var menu in menuSettingTemplate)
            {
                var roleMenuLevel1 = SetMenuDisplay(menu, roleMenus);

                foreach (var subMenu in menu.SubMenus)
                {
                    var roleMenuLevel2 = SetMenuDisplay(subMenu, roleMenuLevel1?.SubMenus);

                    foreach (var permission in subMenu.SubMenus)
                    {
                        SetMenuDisplay(permission, roleMenuLevel2?.SubMenus);
                    }
                }
            }

            return menuSettingTemplate;
        }

        private MenuSetting SetMenuDisplay(MenuSetting menuTemplate, MenuSetting[] roleMenus)
        {
            var roleMenuLevel1 = roleMenus?.FirstOrDefault(p => p.Code == menuTemplate.Code);
            menuTemplate.IsDisplay = roleMenuLevel1?.IsDisplay ?? false;
            return roleMenuLevel1;
        }
    }
}
