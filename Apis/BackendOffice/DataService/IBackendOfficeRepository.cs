using Microsoft.EntityFrameworkCore;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Apis.TGS.Apis.BackendOffice.DataService
{
    public interface IBackendOfficeRepository : IBaseRepository
    {
        DbSet<User> Users { get; set; }

        DbSet<Role> Roles { get; set; }

        DbSet<Menu> Menus { get; set; }

        DbSet<Permission> Permissions { get; set; }

        DbSet<RoleMenu> RoleMenus { get; set; }

        DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
    }
}
