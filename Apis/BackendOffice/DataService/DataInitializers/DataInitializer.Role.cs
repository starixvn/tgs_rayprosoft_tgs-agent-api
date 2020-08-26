using System;
using TGS.Apis.BackendOffice.DataService.Enums;

namespace TGS.Apis.BackendOffice.DataService.DataInitializers
{
    public partial class DataInitializer
    {
        internal interface IRole
        {
            Guid Id { get; }
            string Name { get; }
            string Description { get; }
            RoleType RoleType { get; }
            IRoleMenuData[] RoleMenus { get; }
        }

        internal static class Roles
        {
            internal static Guid Admin = new Guid("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44E");
        }

        internal class BaseRole
        {
            protected IRoleMenuData[] WithoutSubMenus = { };

            protected IRoleMenuData[] GetAdminRoleMenus()
            {
                var roleId = Roles.Admin;

                var resellerMenuId = Guid.NewGuid();
                var resellersMenu = new RoleMenuData(Guid.NewGuid(), resellerMenuId, roleId, 1, ResellersMenu, WithoutSubMenus);
                var brandsMenu = new RoleMenuData(Guid.NewGuid(), resellerMenuId, roleId, 2, BrandsMenu, WithoutSubMenus);

                var resellerMenu = new RoleMenuData(resellerMenuId, null, roleId, 1, ResellerMenu, new IRoleMenuData[] { resellersMenu, brandsMenu });

                var gameMenuId = Guid.NewGuid();
                var gameProviderMenu = new RoleMenuData(Guid.NewGuid(), gameMenuId, roleId, 1, GameProvidersMenu, WithoutSubMenus);
                var gameMenu = new RoleMenuData(gameMenuId, null, roleId, 2, GameMenu, new IRoleMenuData[] { gameProviderMenu });

                var logMenuId = Guid.NewGuid();
                var adminLogMenu = new RoleMenuData(Guid.NewGuid(), logMenuId, roleId, 1, AdminLogMenu, WithoutSubMenus);
                var apiLogMenu = new RoleMenuData(Guid.NewGuid(), logMenuId, roleId, 2, ApiLogMenu, WithoutSubMenus);
                var systemLog = new RoleMenuData(Guid.NewGuid(), logMenuId, roleId, 3, SystemLogMenu, WithoutSubMenus);
                var logMenu = new RoleMenuData(logMenuId, null, roleId, 3, LogMenu, new IRoleMenuData[] { adminLogMenu, apiLogMenu, systemLog });

                var reportMenuId = Guid.NewGuid();
                var overallMenu = new RoleMenuData(Guid.NewGuid(), reportMenuId, roleId, 1, OverallMenu, WithoutSubMenus);
                var reportGpMenu = new RoleMenuData(Guid.NewGuid(), reportMenuId, roleId, 2, GameProviderMenu, WithoutSubMenus);
                var reportMenu = new RoleMenuData(reportMenuId, null, roleId, 4, ReportMenu, new IRoleMenuData[] { overallMenu, reportGpMenu, });

                var systemMenuId = Guid.NewGuid();
                var usersMenu = new RoleMenuData(Guid.NewGuid(), systemMenuId, roleId, 1, UsersMenu, WithoutSubMenus);
                var currenciesMenu = new RoleMenuData(Guid.NewGuid(), systemMenuId, roleId, 2, CurrenciesMenu, WithoutSubMenus);
                var languagesMenu = new RoleMenuData(Guid.NewGuid(), systemMenuId, roleId, 3, LanguagesMenu, WithoutSubMenus);
                var configsMenu = new RoleMenuData(Guid.NewGuid(), systemMenuId, roleId, 4, ConfigsMenu, WithoutSubMenus);
                var whitelistMenu = new RoleMenuData(Guid.NewGuid(), systemMenuId, roleId, 5, WhiteListMenu, WithoutSubMenus);
                var systemMenu = new RoleMenuData(systemMenuId, null, roleId, 5, SystemMenu,
                    new IRoleMenuData[]
                    {
                        usersMenu, currenciesMenu, languagesMenu, configsMenu, whitelistMenu
                    });

                var rolePermissionMenuId = Guid.NewGuid();
                var rolesMenu = new RoleMenuData(Guid.NewGuid(), rolePermissionMenuId, roleId, 1, RolesMenu, WithoutSubMenus);
                var menusMenu = new RoleMenuData(Guid.NewGuid(), rolePermissionMenuId, roleId, 2, MenuMenu, WithoutSubMenus);
                var permissionMenu = new RoleMenuData(Guid.NewGuid(), rolePermissionMenuId, roleId, 3, PermissionMenu, WithoutSubMenus);

                var rolePermissionMenu = new RoleMenuData(rolePermissionMenuId, null, roleId, 6, RolePermissionMenu,
                    new IRoleMenuData[]
                    {
                        rolesMenu,menusMenu, permissionMenu
                    });

                return new IRoleMenuData[] { resellerMenu, gameMenu, logMenu, reportMenu, systemMenu, rolePermissionMenu };
            }

            protected IRoleMenuData[] GetBrandRoleMenus()
            {
                return new IRoleMenuData[] { };
            }

            protected IRoleMenuData[] GetResellerAdminRoleMenus()
            {
                return new IRoleMenuData[] { };
            }
        }

        internal class AdminRole : BaseRole, IRole
        {
            Guid IRole.Id => new Guid("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44E");
            string IRole.Name => "Admin";
            string IRole.Description => "It is used for admin users.";
            RoleType IRole.RoleType => RoleType.Admin;
            IRoleMenuData[] IRole.RoleMenus => GetAdminRoleMenus();
        }


        internal class ResellerAdminRole : BaseRole, IRole
        {
            Guid IRole.Id => new Guid("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44A");
            string IRole.Name => "Reseller Admin";
            string IRole.Description => "It is used for reseller admin.";
            RoleType IRole.RoleType => RoleType.Reseller;
            IRoleMenuData[] IRole.RoleMenus => GetResellerAdminRoleMenus();
        }

        internal class BrandRole : BaseRole, IRole
        {
            Guid IRole.Id => new Guid("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44B");
            string IRole.Name => "Brand";
            string IRole.Description => "It is used for brands.";
            RoleType IRole.RoleType => RoleType.Licensee;
            IRoleMenuData[] IRole.RoleMenus => GetBrandRoleMenus();
        }
    }
}