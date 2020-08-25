using System;

namespace TGS.Apis.TGS.Apis.BackendOffice.DataService.DataInitializers
{
    public partial class DataInitializer
    {
        internal interface IRoleMenuData
        {
            Guid? ParentId { get; }
            Guid RoleMenuId { get; }
            IMenu Menu { get; }
            Guid RoleId { get; }
            int DisplayOrder { get; }
            bool IsDisplay { get; }
            IRoleMenuData[] SubMenus { get; }
        }

        internal class RoleMenuData : IRoleMenuData
        {
            private readonly IRoleMenuData[] _subMenus;
            private readonly IMenu _menu;
            private readonly Guid _roleMenuId;
            private readonly Guid? _parentId;
            private readonly Guid _roleId;
            private readonly int _displayOrder;
            private readonly bool _isDisplay;

            public RoleMenuData(Guid roleMenuId, Guid? parentId, Guid roleId, int displayOrder, IMenu menu, IRoleMenuData[] subMenus, bool isDisplay = true)
            {
                _roleMenuId = roleMenuId;
                _roleId = roleId;
                _displayOrder = displayOrder;
                _menu = menu;
                _subMenus = subMenus;
                _parentId = parentId;
                _isDisplay = isDisplay;
            }

            bool IRoleMenuData.IsDisplay => _isDisplay;
            Guid IRoleMenuData.RoleId => _roleId;
            int IRoleMenuData.DisplayOrder => _displayOrder;
            IMenu IRoleMenuData.Menu => _menu;
            IRoleMenuData[] IRoleMenuData.SubMenus => _subMenus;
            Guid IRoleMenuData.RoleMenuId => _roleMenuId;
            Guid? IRoleMenuData.ParentId => _parentId;
        }
    }
}