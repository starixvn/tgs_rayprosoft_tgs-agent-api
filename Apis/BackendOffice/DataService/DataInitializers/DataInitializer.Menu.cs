using System;

namespace TGS.Apis.BackendOffice.DataService.DataInitializers
{
    public partial class DataInitializer
    {
        static readonly Guid PerCreateId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D5021");
        static readonly Guid PerUpdateId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D5020");
        static readonly Guid PerDeleteId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D5019");
        static readonly Guid PerManageStatusId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D5017");
        static readonly Guid PerExportId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D4016");
        static readonly Guid PerSubmitId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D9494");
        static readonly Guid PerViewId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D9495");
        static readonly Guid PerImportId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518F9496");
        static readonly Guid PerCanManageArchiveStatusId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D5030");
        static readonly Guid PerCanManageConfigStatusId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D5031");
        static readonly Guid PerCanViewGamesId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D5032");
        static readonly Guid PerCanViewBetHistoryId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D5111");
        static readonly Guid PerCanEditCredentialId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D5114");
        static readonly Guid PerCanManageMenuId = new Guid("F5D8C1F6-7634-41CC-A11C-D2A8518D6999");

        static readonly Guid[] ManageRolePermissions = { PerViewId, PerCreateId, PerUpdateId, PerCanManageMenuId, PerManageStatusId, PerExportId, PerImportId };
        static readonly Guid[] ManagePermissionPermissions = { PerViewId, PerCreateId, PerUpdateId, PerDeleteId };
        static readonly Guid[] ManageWithStatusPermissions = { PerViewId, PerCreateId, PerUpdateId, PerManageStatusId };
        static readonly Guid[] ManageBrandPermissions = { PerViewId, PerCreateId, PerUpdateId, PerManageStatusId };
        static readonly Guid[] ManageGameProviderPermissions = { PerViewId, PerCreateId, PerUpdateId, PerCanEditCredentialId, PerManageStatusId, PerCanManageArchiveStatusId, PerCanManageConfigStatusId, PerCanViewGamesId };
        static readonly Guid[] CreateAndUpdatePermissions = { PerViewId, PerCreateId, PerUpdateId };
        static readonly Guid[] ReportGameProviderPermissions = { PerViewId, PerExportId, PerCanViewBetHistoryId };
        static readonly Guid[] ReportOverAllPermissions = { PerViewId, PerExportId, PerCanViewBetHistoryId };
        static readonly Guid[] EmptyPermissions = { };
        static readonly Guid[] ManagePermissions = { PerViewId, PerCreateId, PerUpdateId, PerManageStatusId };
        static readonly Guid[] ViewPermissions = { PerViewId };
        static readonly Guid[] WhiteListPermissions = { PerViewId, PerCreateId, PerUpdateId, PerExportId };
        static readonly Guid[] ManageResellersPermissions = { PerViewId, PerCreateId, PerUpdateId, PerManageStatusId, PerCanEditCredentialId };

        static readonly IMenu AdminMenu = new Menu_(EmptyPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB259"), "fa-certificate", "Admin", "Admin", "Resources.UI.Partial.Menu.Menu.LinkAdmin");
        static readonly IMenu AdminUsersMenu = new Menu_(ManageWithStatusPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB289"), string.Empty, "AdminUsers", "Admin Users", "Resources.UI.Partial.Menu.Menu.LinkUsers");
        static readonly IMenu RolesMenu = new Menu_(ManageRolePermissions, new Guid("ADE95FB2-2348-41AC-B899-EF8A95D10103"), string.Empty, "Roles", "Roles", "Resources.UI.Partial.Menu.Menu.LinkRoles");
        static readonly IMenu PermissionMenu = new Menu_(ManagePermissionPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB389"), string.Empty, "Permission", "Permission", "Resources.UI.Partial.Menu.Menu.LinkPermission");
        static readonly IMenu RolePermissionMenu = new Menu_(EmptyPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB312"), "fa-certificate", "RolePermission", "Role And Permission", "Resources.UI.Partial.Menu.Menu.MenuTextRoleAndPermission");

        static readonly IMenu ResellersMenu = new Menu_(ManageResellersPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB261"), string.Empty, "Resellers", "Resellers", "Resources.UI.Partial.Menu.Menu.LinkResellers");
        static readonly IMenu ResellerMenu = new Menu_(EmptyPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB260"), "fa-certificate", "Reseller", "Reseller", "Resources.UI.Partial.Menu.Menu.MenuTextReseller");
        static readonly IMenu BrandsMenu = new Menu_(ManageBrandPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB266"), string.Empty, "Brands", "Brands", "Resources.UI.Partial.Menu.Menu.LinkBrands");

        static readonly IMenu GameMenu = new Menu_(EmptyPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB267"), "fa-gamepad", "Game", "Game", "Resources.UI.Partial.Menu.Menu.MenuTextGame");
        static readonly IMenu GameProvidersMenu = new Menu_(ManageGameProviderPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB269"), string.Empty, "GameProviders", "Game Providers", "Resources.UI.Partial.Menu.Menu.LinkGameProviders");

        static readonly IMenu LogMenu = new Menu_(EmptyPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB277"), "fa-file-text-o", "Log", "Log", "Resources.UI.Partial.Menu.Menu.MenuTextLog");
        static readonly IMenu AdminLogMenu = new Menu_(ViewPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB278"), string.Empty, "AdminLog", "Admin", "Resources.UI.Partial.Menu.Menu.LinkAdmin");
        static readonly IMenu ApiLogMenu = new Menu_(ViewPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB279"), string.Empty, "ApiLog", "API", "Resources.UI.Partial.Menu.Menu.LinkAPI");
        static readonly IMenu SystemLogMenu = new Menu_(ViewPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB280"), string.Empty, "SystemLog", "System", "Resources.UI.Partial.Menu.Menu.LinkSystem");

        static readonly IMenu ReportMenu = new Menu_(EmptyPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB281"), "fa-bar-chart", "Report", "Report", "Resources.UI.Partial.Menu.Menu.MenuTextReport");
        static readonly IMenu OverallMenu = new Menu_(ReportOverAllPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB282"), string.Empty, "Overall", "Overall", "Resources.UI.Partial.Menu.Menu.LinkOverall");
        static readonly IMenu GameProviderMenu = new Menu_(ReportGameProviderPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB283"), string.Empty, "ReportGameProvider", "GameProvider", "Resources.UI.Partial.Menu.Menu.LinkGameProvider");

        static readonly IMenu SystemMenu = new Menu_(EmptyPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB288"), "fa-cog", "System", "System", "Resources.UI.Partial.Menu.Menu.MenuTextSystem");
        static readonly IMenu CurrenciesMenu = new Menu_(CreateAndUpdatePermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB291"), string.Empty, "Currencies", "Currencies", "Resources.UI.Partial.Menu.Menu.LinkCurrencies");
        static readonly IMenu LanguagesMenu = new Menu_(CreateAndUpdatePermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB292"), string.Empty, "Languages", "Languages", "Resources.UI.Partial.Menu.Menu.LinkLanguages");
        static readonly IMenu ConfigsMenu = new Menu_(ManagePermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB302"), string.Empty, "Configurations", "Configurations", "Resources.UI.Partial.Menu.Menu.LinkApplicationConfigurations");
        static readonly IMenu WhiteListMenu = new Menu_(WhiteListPermissions, new Guid("5F27A355-42EC-4E92-8F1C-3E3F5D6FB303"), string.Empty, "WhilelistIPs", "WhileLists IPs", "Resources.UI.Partial.Menu.Menu.LinkWhitelists");

        static readonly IMenu[] Menus =
        {
            AdminMenu, RolesMenu, RolePermissionMenu,
            ResellersMenu, ResellerMenu, BrandsMenu,
            GameMenu, GameProvidersMenu,
            LogMenu,AdminLogMenu,ApiLogMenu,SystemLogMenu,
            ReportMenu, OverallMenu, GameProviderMenu,
            SystemMenu, AdminUsersMenu, CurrenciesMenu,LanguagesMenu,ConfigsMenu,WhiteListMenu,
        };

        internal interface IMenu
        {
            Guid MenuId { get; }
            string Icon { get; }
            string Code { get; }
            string Name { get; }
            string Translation { get; }
            Guid[] Permissions { get; }
        }

        // ReSharper disable once InconsistentNaming
        internal class Menu_ : IMenu
        {
            readonly Guid _menuId;
            readonly string _code;
            readonly string _name;
            readonly string _icon;
            readonly string _translation;
            readonly Guid[] _permissions;
            public Menu_(Guid[] permissions, Guid menuId, string icon, string code, string name, string translation)
            {
                _menuId = menuId;
                _icon = icon;
                _code = code;
                _name = name;
                _translation = translation;
                _permissions = permissions;
            }

            Guid IMenu.MenuId => _menuId;
            string IMenu.Icon => _icon;
            string IMenu.Code => _code;
            string IMenu.Name => _name;
            string IMenu.Translation => _translation;
            Guid[] IMenu.Permissions => _permissions;
        }
    }
}