using System;
using System.Collections.Generic;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Apis.BackendOffice.DataService
{
    public interface IBackendOfficeRepository : IBaseRepository
    {
        DbSet<User> Users { get; set; }

        DbSet<Role> Roles { get; set; }

        DbSet<Menu> Menus { get; set; }

        DbSet<Permission> Permissions { get; set; }

        DbSet<RoleMenu> RoleMenus { get; set; }

        DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }

        void BulkInsert<T>(IList<T> entities, BulkConfig bulkConfig = null, Action<decimal> progress = null) where T : class;
    }
}
