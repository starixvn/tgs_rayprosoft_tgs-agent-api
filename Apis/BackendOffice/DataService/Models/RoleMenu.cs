using System;
using System.Collections.Generic;

namespace TGS.Apis.BackendOffice.DataService.Models
{
    public class RoleMenu
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public Role Role { get; set; }

        public Guid MenuId { get; set; }

        public Menu Menu { get; set; }

        public int DisplayOrder { get; set; }

        public Guid? ParentId { get; set; }

        public bool IsDisplay { get; set; }

        public virtual ICollection<RoleMenuPermission> Permissions { get; set; }

        public virtual RoleMenu Parent { get; set; }

        public virtual ICollection<RoleMenu> SubMenus { get; set; }
    }
}
