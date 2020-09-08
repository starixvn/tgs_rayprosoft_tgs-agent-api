using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Roles;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Apis.BackendOffice.Services;
using TGS.Infrastructures.Extensions;
using TGS.Infrastructures.Json;

namespace TGS.Apis.BackendOffice.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BackendOfficeControllerBase
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

        [HttpGet("{roleId}"), Description("Get a role")]
        public async Task<GetRoleResponse> GetRoleAsync(Guid roleId)
        {
            var role = await _roleQueries.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return CreateInvalidResponse<GetRoleResponse>($"{Resources.Role.NotFound}");
            }
            var roleModel = new RoleModel
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.DisplayName,
                Description = role.Description,
                Menus = await GetMenuSettingAsync(role.MenuSetting),
                CreatedOn = role.CreatedOn,
                UpdatedOn = role.UpdatedOn,
                CreatedByUsername = role.CreatedByUsername,
                UpdatedByUsername = role.CreatedByUsername
            };

            return new GetRoleResponse
            {
                Valid = true,
                Role = roleModel
            };
        }

        [HttpPut("{roleId}"), Description("Update role")]
        public async Task<GetRoleResponse> UpdateRoleAsync(Guid roleId,
            [FromQuery] UpdateRoleRequest request)
        {
            var role = await _roleQueries.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return CreateInvalidResponse<GetRoleResponse>($"{Resources.Role.NotFound}");
            }

            if (request.Name != role.Name && await _roleQueries.NameExistsAsync(request.Name))
            {
                return CreateInvalidResponse<GetRoleResponse>($"{Resources.Role.NameExistd}");
            }

            role.Name = request.Name;
            role.MenuSetting = _jsonSerializationProvider.SerializeToString(request.Menus);
            role.UpdatedByUsername = Username;
            role.UpdatedOn = DateTimeOffset.Now;

            await _roleCommands.UpdateRoleAsync(role);
            return CreateValidResponse<GetRoleResponse>();
        }

        [HttpDelete("{roleId}"), Description("Delete a role")]
        public async Task<GetRoleResponse> DeleteRoleAsync(Guid roleId)
        {
            var role = await _roleQueries.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NotFound);
            }

            if (role.Users.Any())
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.CannotDeleteRole);
            }

            await _roleCommands.DeleteRoleAsync(role);
            return CreateValidResponse<GetRoleResponse>();
        }

        [HttpPost("{roleId}/copy"), Description("Copy role")]
        public async Task<GetRoleResponse> CopyRoleAsync(Guid roleId, [FromBody] CopyRoleRequest request)
        {
            var role = await _roleQueries.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NotFound);
            }

            if (request.Name == role.Name || await _roleQueries.NameExistsAsync(request.Name))
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NameExistd);
            }

            var newRole = CreateRole(
                request.Name,
                role.DisplayName,
                role.Description,
                role.MenuSetting);

            await _roleCommands.CreateRoleAsync(newRole);
            return CreateValidResponse<GetRoleResponse>();
        }

        [HttpPost, Description("Create a role")]
        public async Task<GetRoleResponse> CreateRoleAsync([FromBody] CreateRoleRequest request)
        {
            if (await _roleQueries.NameExistsAsync(request.Name))
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NameExistd);
            }

            var role = CreateRole(
                request.Name,
                request.DisplayName,
                request.Description,
                _jsonSerializationProvider.SerializeToString(request.Menus));

            await _roleCommands.CreateRoleAsync(role);
            return CreateValidResponse<GetRoleResponse>();
        }

        private Role CreateRole(string name, string displayName, string description, string menuSetting)
        {
            return new Role
            {
                Id = GuidExtensions.NewSequentialGuid(),
                Name = name,
                Description = description,
                DisplayName = displayName,
                MenuSetting = menuSetting,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };
        }

        private async Task<MenuSetting[]> GetMenuSettingAsync(string menuSetting)
        {
            var setting = await _settingQueries.GetSettingByKeyAsync("Menus");
            var menuSettingTemplate = GetMenuSettingByString(setting.Value);
            var roleMenus = GetMenuSettingByString(menuSetting);

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

        private MenuSetting[] GetMenuSettingByString(string setting)
        {
            return _jsonSerializationProvider.DeserializeFromString<MenuSetting[]>(setting);
        }

        private MenuSetting SetMenuDisplay(MenuSetting menuTemplate, MenuSetting[] roleMenus)
        {
            var roleMenuLevel1 = roleMenus?.FirstOrDefault(p => p.Code == menuTemplate.Code);
            menuTemplate.IsDisplay = roleMenuLevel1?.IsDisplay ?? false;
            return roleMenuLevel1;
        }
    }
}
