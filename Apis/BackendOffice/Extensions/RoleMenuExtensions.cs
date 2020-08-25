using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using TGS.Apis.TGS.Apis.BackendOffice.Controllers.Messages.UserAuthentication;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.TGS.Apis.BackendOffice.Extensions
{
    public static class RoleMenuExtensions
    {
        public static RoleMenuViewModel[] CreateViewMenus(this IEnumerable<RoleMenu> menus)
        {
            return menus.Where(m => m.IsDisplay).Select(menu => new RoleMenuViewModel
            {
                Icon = menu.Menu.Icon,
                Code = menu.Menu.Code,
                Name = menu.Menu.Name,
                Translation = menu.Menu.Translation,
                DisplayOrder = menu.DisplayOrder,
                SubMenus = menu.SubMenus?
                    .Where(sm => sm.Permissions.Any(p => p.Permission.Name.Equals("View") && p.IsDisplay))
                    .CreateRoleSubMenuViewModels()
                    .ToList()
            }).OrderBy(m => m.DisplayOrder).ToArray();
        }

        private static IEnumerable<RoleSubMenuViewModel> CreateRoleSubMenuViewModels(this IEnumerable<RoleMenu> menus)
        {
            return menus?.Where(m => m.IsDisplay).Select(menu => new RoleSubMenuViewModel
            {
                Code = menu.Menu.Code,
                Name = menu.Menu.Name,
                Translation = menu.Menu.Translation,
                DisplayOrder = menu.DisplayOrder,
                Permissions = menu.Permissions?.CreateDynamicProperties()
            }).OrderBy(p => p.DisplayOrder);
        }

        private static dynamic CreateDynamicProperties(this IEnumerable<RoleMenuPermission> properties)
        {
            var dynamicObject = new ExpandoObject() as IDictionary<string, object>;
            foreach (var property in properties)
            {
                dynamicObject.Add(property.Permission.Code, property.IsDisplay);
            }
            return dynamicObject;
        }
    }
}
