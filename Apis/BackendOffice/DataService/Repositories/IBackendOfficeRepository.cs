using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public interface IBackendOfficeRepository : IBaseRepository
    {
        DbSet<User> Users { get; set; }

        DbSet<Role> Roles { get; set; }

        DbSet<Setting> Settings { get; set; }

        DbSet<Department> Departments { get; set; }

        EntityEntry<T> Entry<T>(T entity) where T : class;

        DbSet<T> Set<T>() where T : class;
    }
}
