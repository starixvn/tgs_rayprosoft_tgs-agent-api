using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Queries
{
    public interface IRoleQueries
    {
        IQueryable<Role> GetRoleQueries();
        Task<Role> GetRoleByIdAsync(Guid roleId);

    }

    public sealed class RoleQueries : IRoleQueries
    {
        private readonly IBackendOfficeRepository _repository;

        public RoleQueries(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        IQueryable<Role> IRoleQueries.GetRoleQueries()
        {
            return _repository.Roles;
        }

        Task<Role> IRoleQueries.GetRoleByIdAsync(Guid roleId)
        {
            return _repository.Roles
                .FirstOrDefaultAsync(a => a.Id == roleId);
        }
    }
}
