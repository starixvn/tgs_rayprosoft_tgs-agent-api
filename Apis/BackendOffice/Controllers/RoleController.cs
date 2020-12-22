using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Roles;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Services;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseContextController
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ITgsCommands<Role> _roleCommands;
        private readonly ITgsQueries<Role> _roleQueries;
        private readonly ITgsQueries<Setting> _settingQueries;

        public RoleController(
            ITgsQueries<Role> roleQueries,
            ITgsQueries<Setting> settingQueries,
            ITgsCommands<Role> roleCommands,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _roleQueries = roleQueries;
            _roleCommands = roleCommands;
            _settingQueries = settingQueries;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        [HttpGet]
        [Description("Get roles")]
        public Task<PagedResponse<RoleModel>> GetListAsync([FromQuery] GetRolesRequest request)
        {
            return _roleQueries.GetQueries()
                .Where(request.IsAdmin, x => x.IsAdmin == request.IsAdmin)
                .Where(ResellerId != null, r => r.ResellerId == ResellerId)
                .Where(LicenseeId != null, l => l.LicenseeId == LicenseeId)
                .Where(BrandId != null, l => l.BrandId == BrandId)
                .Where(request.Name, p => p.Name.Contains(request.Name))
                .Select(role => new RoleModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    DisplayName = role.DisplayName,
                    Description = role.Description,
                    IsAdmin = role.IsAdmin,
                    CreatedOn = role.CreatedOn,
                    UpdatedOn = role.UpdatedOn,
                    CreatedByUsername = role.CreatedByUsername,
                    UpdatedByUsername = role.UpdatedByUsername
                })
                .CreatePagedResponseAsync(request);
        }

        [HttpGet("{roleId}")]
        [Description("Get a role")]
        public async Task<GetRoleResponse> GetSingleAsync(Guid roleId)
        {
            var role = await _roleQueries.GetSingleAsync(p => p.Id == roleId, x => x
                .Where(ResellerId != null, r => r.ResellerId == ResellerId)
                .Where(LicenseeId != null, l => l.LicenseeId == LicenseeId)
                .Where(BrandId != null, l => l.BrandId == BrandId));

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
                IsAdmin = role.IsAdmin,
                Menus = await GetMenuSettingAsync(role.MenuSetting, role.RoleType),
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

        [HttpPut("{roleId}")]
        [Description("Update role")]
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

        [HttpDelete("{roleId}")]
        [Description("Delete a role")]
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

        [HttpPost("{roleId}/copy")]
        [Description("Copy role")]
        public async Task<MessageResponse> CopyAsync(Guid roleId, [FromBody] CopyRoleRequest request)
        {
            var role = await _roleQueries.GetSingleAsync(p => p.Id == roleId);
            if (role == null)
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NotFound);
            }

            if (role.IsAdmin)
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.CannotCopyRootRole);
            }

            if (request.Name == role.Name || await _roleQueries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NameExists);
            }

            var newRole = CreateRole(request.Name, role.DisplayName, role.Description, role.ResellerId,
                role.RoleType, role.LicenseeId, role.BrandId, role.MenuSetting, role.IsAdmin);

            await _roleCommands.CreateAsync(newRole);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost]
        [Description("Create a role")]
        public async Task<MessageResponse> CreateAsync([FromBody] CreateRoleRequest request)
        {
            if (await _roleQueries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NameExists);
            }

            if (request.IsAdmin && request.RoleType == RoleType.Brand &&
                await _roleQueries.GetCountAsync(x => x.IsAdmin && x.BrandId == request.BrandId) > 0)
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.BrandRootExisted);
            }

            if (request.IsAdmin && request.RoleType == RoleType.Licensee &&
                await _roleQueries.GetCountAsync(x => x.IsAdmin && x.LicenseeId == request.LicenseeId) > 0)
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.LicenseeRootExisted);
            }

            if (request.IsAdmin && request.RoleType == RoleType.Reseller &&
                await _roleQueries.GetCountAsync(x => x.IsAdmin && x.ResellerId == request.ResellerId) > 0)
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.ResellerRootExisted);
            }

            if (request.IsAdmin && request.RoleType == RoleType.Admin &&
                await _roleQueries.GetCountAsync(x =>
                    x.IsAdmin && x.ResellerId == null && x.LicenseeId == null && x.BrandId == null) > 0)
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.RootExixted);
            }

            var role = CreateRole(request.Name, request.DisplayName, request.Description, request.ResellerId,
                request.RoleType, request.LicenseeId, request.BrandId,
                _jsonSerializationProvider.SerializeToString(request.Menus), request.IsAdmin);

            await _roleCommands.CreateAsync(role);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost("menus")]
        [Description("Get menus")]
        public async Task<GetMenuResponse> GetMenuAsync(GetMenuRequest request)
        {
            var setting = request.RoleType != RoleType.Admin
                ? (await _roleQueries.GetSingleAsync(x => x.Id == request.RoleId))?.MenuSetting
                : (await _settingQueries.GetSingleAsync(p => p.Key == "Menus"))?.Value;

            if (setting == null)
            {
                return CreateInvalidResponse<GetMenuResponse>(Resources.Role.MenuNotFound);
            }

            var menus = _jsonSerializationProvider.DeserializeFromString<MenuSetting[]>(setting);

            return new GetMenuResponse
            {
                Valid = true,
                Menus = menus
            };
        }

        private Role CreateRole(string name, string displayName, string description, Guid? resellerId,
            RoleType roleType, Guid? licenseeId, Guid? brandId, string menuSetting, bool IsAdmin) =>
            new()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                IsAdmin = IsAdmin,
                Name = name,
                Description = description ?? name,
                DisplayName = displayName ?? name,
                MenuSetting = menuSetting,
                RoleType = roleType,
                ResellerId = resellerId,
                LicenseeId = licenseeId,
                BrandId = brandId,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

        private async Task<MenuSetting[]> GetMenuSettingAsync(string menuSetting, RoleType type)
        {
            if (type != RoleType.Admin)
            {
                return GetMenuSettingByString(menuSetting);
            }

            var setting = await _settingQueries.GetSingleAsync(p => p.Key == "Menus");
            var menuSettingTemplate = GetMenuSettingByString(setting.Value);

            return menuSettingTemplate;
        }

        private MenuSetting[] GetMenuSettingByString(string setting) =>
            _jsonSerializationProvider.DeserializeFromString<MenuSetting[]>(setting);
    }
}