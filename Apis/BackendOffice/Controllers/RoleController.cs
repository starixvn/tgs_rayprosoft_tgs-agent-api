using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Roles;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseUserContextController
    {
        private readonly IBackendOfficeQueries<Role> _roleQueries;
        private readonly IBackendOfficeQueries<Setting> _settingQueries;
        private readonly IBackendOfficeCommands<Role> _roleCommands;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public RoleController(
            IBackendOfficeQueries<Role> roleQueries,
            IBackendOfficeQueries<Setting> settingQueries,
            IBackendOfficeCommands<Role> roleCommands,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _roleQueries = roleQueries;
            _roleCommands = roleCommands;
            _settingQueries = settingQueries;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        [HttpGet, Description("Get roles")]
        public Task<PagedResponse<RoleModel>> GetListAsync([FromQuery] GetRolesRequest request)
        {
            return _roleQueries.GetQueries()
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
                    UpdatedByUsername = role.UpdatedByUsername

                })
                .CreatePagedResponseAsync(request);
        }

        [HttpGet("{roleId}"), Description("Get a role")]
        public async Task<GetRoleResponse> GetSingleAsync(Guid roleId)
        {
            var role = await _roleQueries.GetSingleAsync(p => p.Id == roleId);
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
                UpdatedByUsername = role.UpdatedByUsername
            };

            return new GetRoleResponse
            {
                Valid = true,
                Role = roleModel
            };
        }

        [HttpPut("{roleId}"), Description("Update role")]
        public async Task<MessageResponse> UpdateAsync(Guid roleId,
            [FromBody] UpdateRoleRequest request)
        {
            var role = await _roleQueries.GetSingleAsync(p => p.Id == roleId);
            if (role == null)
            {
                return CreateInvalidResponse<GetRoleResponse>($"{Resources.Role.NotFound}");
            }

            if (request.Name != role.Name && await _roleQueries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<GetRoleResponse>($"{Resources.Role.NameExists}");
            }

            role.Name = request.Name;
            role.MenuSetting = _jsonSerializationProvider.SerializeToString(request.Menus);
            role.UpdatedByUsername = Username;
            role.UpdatedOn = DateTimeOffset.Now;


            await _roleCommands.UpdateAsync(role);

            return CreateValidResponse<MessageResponse>();
        }

        [HttpDelete("{roleId}"), Description("Delete a role")]
        public async Task<MessageResponse> DeleteAsync(Guid roleId)
        {
            var role = await _roleQueries.GetSingleAsync(p => p.Id == roleId, s => s.Include(x => x.Users));
            if (role == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Role.NotFound);
            }

            if (role.Users.Any())
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Role.CannotDeleteRole);
            }

            await _roleCommands.DeleteAsync(role);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost("{roleId}/copy"), Description("Copy role")]
        public async Task<MessageResponse> CopyAsync(Guid roleId, [FromBody] CopyRoleRequest request)
        {
            var role = await _roleQueries.GetSingleAsync(p => p.Id == roleId);
            if (role == null)
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NotFound);
            }

            if (request.Name == role.Name || await _roleQueries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NameExists);
            }

            var newRole = CreateRole(
                request.Name,
                role.DisplayName,
                role.Description,
                role.MenuSetting);

            await _roleCommands.CreateAsync(newRole);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost, Description("Create a role")]
        public async Task<MessageResponse> CreateAsync([FromBody] CreateRoleRequest request)
        {
            if (await _roleQueries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NameExists);
            }

            var role = CreateRole(
                request.Name,
                request.DisplayName,
                request.Description,
                _jsonSerializationProvider.SerializeToString(request.Menus));

            await _roleCommands.CreateAsync(role);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost("menus"), Description("Get menus")]
        public async Task<GetMenuResponse> GetMenuAsync()
        {
            var setting = await _settingQueries.GetSingleAsync(p => p.Key == "Menus");
            if (setting == null)
            {
                return CreateInvalidResponse<GetMenuResponse>(Resources.Role.MenuNotFound);
            }

            var menus = _jsonSerializationProvider.DeserializeFromString<MenuSetting[]>(setting.Value);

            return new GetMenuResponse
            {
                Valid = true,
                Menus = menus
            };
        }

        private Role CreateRole(string name, string displayName, string description, string menuSetting)
        {
            return new Role
            {
                Id = GuidExtensions.NewSequentialGuid(),
                Name = name,
                Description = description ?? string.Empty,
                DisplayName = displayName ?? string.Empty,
                MenuSetting = menuSetting,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };
        }

        private async Task<MenuSetting[]> GetMenuSettingAsync(string menuSetting)
        {
            var setting = await _settingQueries.GetSingleAsync(p => p.Key == "Menus");
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
