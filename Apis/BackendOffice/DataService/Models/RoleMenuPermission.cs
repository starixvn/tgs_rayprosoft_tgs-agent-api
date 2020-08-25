using System;

namespace TGS.Apis.TGS.Apis.BackendOffice.DataService.Models
{
    public class RoleMenuPermission
    {
        public Guid Id { get; set; }

        public Guid RoleMenuId { get; set; }

        public Guid PermissionId { get; set; }

        public bool IsDisplay { get; set; }

        public int DisplayOrder { get; set; }

        public virtual Permission Permission { get; set; }

        public virtual RoleMenu RoleMenu { get; set; }
    }
}
