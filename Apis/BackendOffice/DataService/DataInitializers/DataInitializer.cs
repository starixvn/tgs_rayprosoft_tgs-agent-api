using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Models;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.TGS.Apis.BackendOffice.DataService.DataInitializers
{
    public interface IDataInitializer
    {
        void SetInitialData();
    }

    [ExcludeFromCodeCoverage]
    public sealed partial class DataInitializer : IDataInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private const string DefaultTimeZone = "+0800";
        private const string AdminUsername = "admin.enabled";
        private const string UserPassword = "TgsP@ssw0rd";
        private static readonly Guid AdminUserId = new Guid("5715ABBA-AF3C-4763-94EE-9F2714C3B5A8");

        public DataInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private readonly IRole[] _roles =
        {
            new AdminRole(), new ResellerAdminRole()
        };

        void IDataInitializer.SetInitialData()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            var repository = serviceScope.ServiceProvider.GetRequiredService<IBackendOfficeRepository>();
            if (repository.Permissions.Any())
            {
                return;
            }

            GeneratePermissions(repository);
            GenerateMenus(repository);
            GenerateRoles(repository);
            GenerateAdminUsers(repository);
        }

        private void GenerateMenus(IBackendOfficeRepository repository)
        {
            var menus = Menus.Select(CreateMenu).ToArray();
            repository.BulkInsert(menus);
        }

        private void GenerateRoles(IBackendOfficeRepository repo)
        {
            var roles = CreateRoleWithMenus(_roles);
            var command = @"
INSERT INTO [dbo].[Roles]
    ([Id]
    ,[RoleType]
    ,[Name]
    ,[DisplayName]
    ,[Description]
    ,[Status]
    ,[CreatedOn]
    ,[UpdatedOn]
    ,[CreatedByUsername]
    ,[UpdatedByUsername])
VALUES
    (@id
    ,@roleType
    ,@name
    ,@displayName
    ,@description
    ,@status
    ,SYSDATETIMEOFFSET()
    ,SYSDATETIMEOFFSET()
    ,@createdByUsername
    ,@updatedByUsername)
";
            foreach (var role in roles)
            {
                var args = new[]
                {
                    new SqlParameter { ParameterName = "@id", Value = role.Id, DbType = DbType.Guid },
                    new SqlParameter { ParameterName = "@roleType", Value = (int)role.RoleType, DbType = DbType.Int16 },
                    new SqlParameter { ParameterName = "@name", Value = role.Name, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@displayName", Value = role.Name, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@description", Value = role.DisplayName, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@status", Value = role.Status, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@createdByUsername", Value = role.CreatedByUsername, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@updatedByUsername", Value = role.UpdatedByUsername, DbType = DbType.String }
                };
                repo.ExecuteSqlCommand(command, CommandType.Text, args);
            }


            var mainMenus = roles.SelectMany(p => p.Menus).ToArray();
            repo.BulkInsert(mainMenus);
            var subMenus = mainMenus.SelectMany(p => p.SubMenus).ToArray();
            repo.BulkInsert(subMenus);
            var permissions = subMenus.SelectMany(p => p.Permissions).ToArray();
            repo.BulkInsert(permissions);
        }


        private static void GeneratePermissions(IBackendOfficeRepository repository)
        {
            var permissions = new List<Permission>
            {
                CreatePermission(PerCreateId, "Create", "Create"),
                CreatePermission(PerUpdateId, "Update", "Update"),
                CreatePermission(PerDeleteId, "Delete", "Delete"),
                CreatePermission(PerManageStatusId,"ManageStatus","Manage Status"),
                CreatePermission(PerExportId,"Export","Export"),
                CreatePermission(PerSubmitId,"Submit","Submit"),
                CreatePermission(PerViewId,"View","View"),
                CreatePermission(PerImportId,"Import","Import"),
                CreatePermission(PerCanManageArchiveStatusId,"CanManageArchiveStatus","Can Manage Archive Status"),
                CreatePermission(PerCanManageConfigStatusId,"CanManageConfigStatus","Can Manage Config Status"),
                CreatePermission(PerCanViewGamesId,"CanViewGames","Can View Games"),
                CreatePermission(PerCanViewBetHistoryId,"CanViewBetHistory","Can View Bet History"),
                CreatePermission(PerCanEditCredentialId,"CanEditCredential","Can Edit Credential"),
                CreatePermission(PerCanManageMenuId,"CanManageMenu","Can Manage Menu"),
            };

            repository.BulkInsert(permissions);
        }

        private static Permission CreatePermission(Guid permissionId, string code, string name)
        {
            return new Permission
            {
                Id = permissionId,
                Code = code,
                Name = name,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername
            };
        }

        private static Menu CreateMenu(IMenu menu)
        {
            return new Menu
            {
                Id = menu.MenuId,
                Code = menu.Code,
                Name = menu.Name,
                Icon = menu.Icon,
                Translation = menu.Translation,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername
            };
        }

        private static Role[] CreateRoleWithMenus(IRole[] roles)
        {
            return roles.Select(role => new Role
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.Name,
                Description = role.Description,
                Status = RoleStatus.Enabled,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                Menus = GetRoleMenus(role),
                RoleType = role.RoleType
            }).ToArray();
        }

        private static RoleMenu[] GetRoleMenus(IRole role)
        {
            var roleMenus = new List<RoleMenu>();
            foreach (var roleMenu in role.RoleMenus)
            {
                var roleMainMenu = new RoleMenu
                {
                    Id = roleMenu.RoleMenuId,
                    RoleId = role.Id,
                    MenuId = roleMenu.Menu.MenuId,
                    DisplayOrder = roleMenu.DisplayOrder,
                    IsDisplay = true
                };

                var roleSubMenus = new List<RoleMenu>();
                foreach (var subMenu in roleMenu.SubMenus)
                {
                    roleSubMenus.Add(new RoleMenu
                    {
                        Id = subMenu.RoleMenuId,
                        RoleId = role.Id,
                        MenuId = roleMenu.Menu.MenuId,
                        ParentId = roleMenu.ParentId,
                        DisplayOrder = roleMenu.DisplayOrder,
                        Permissions = subMenu.Menu.Permissions.Select((permission, index) => new RoleMenuPermission
                        {
                            Id = GuidExtensions.NewSequentialGuid(),
                            RoleMenuId = subMenu.RoleMenuId,
                            PermissionId = permission,
                            Selected = subMenu.Selected,
                            DisplayOrder = index + 1
                        }).ToArray()
                    });
                }

                roleMainMenu.SubMenus = roleSubMenus.ToArray();
                roleMenus.Add(roleMainMenu);
            }
            return roleMenus.ToArray();
        }

        private void GenerateAdminUsers(IBackendOfficeRepository repository)
        {
            string[] usernames =
            {
                "hank.wu", "lester.wei", "sam.chuang","houdini.sheng"
            };

            var users = new List<User>
            {
                // Administrator
                CreateUser("admin.disabled", UserStatus.Disabled, Roles.Admin),
                CreateUser("admin.enabled", UserStatus.Enabled, Roles.Admin, AdminUserId),
                CreateUser("admin.archived", UserStatus.Archived, Roles.Admin),
                CreateUser("admin.locked", UserStatus.Locked, Roles.Admin),
            };

            users.AddRange(usernames.Select(username => CreateUser(username, UserStatus.Enabled, Roles.Admin)));

            //var brandIds = UgsData.OureaLicensee.BrandIds
            //    .Union(UgsData.Sc88Licensee.BrandIds)
            //    .Union(UgsData.SunbetLicensee.BrandIds)
            //    .Union(UgsData.TgpLicensee.BrandIds)
            //    .ToArray();
            //var userBrands = users
            //    .Where(x =>
            //    {
            //        var role = _roles.First(r => r.Id == x.RoleId);
            //        return role.RestrictionType.HasFlag(RestrictionType.Brand);
            //    })
            //    .SelectMany(user => brandIds.Select(id => new UserBrand
            //    {
            //        BrandId = id,
            //        UserId = user.Id
            //    })).ToArray();

            //repository.BulkInsert(users.ToArray());
            //repository.BulkInsert(userBrands);
        }
    }
}