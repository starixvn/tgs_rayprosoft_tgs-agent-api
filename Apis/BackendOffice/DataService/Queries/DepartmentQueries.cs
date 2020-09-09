using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Queries
{
    public interface IDepartmentQueries
    {
        IQueryable<Department> GetDepartmentQueries();
        Task<Department> GetDepartmentByIdAsync(Guid departmentId);
        Task<bool> NameExistsAsync(string name);
    }

    public sealed class DepartmentQueries : IDepartmentQueries
    {
        private readonly IBackendOfficeRepository _repository;

        public DepartmentQueries(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        IQueryable<Department> IDepartmentQueries.GetDepartmentQueries()
        {
            return _repository.Departments;
        }

        Task<Department> IDepartmentQueries.GetDepartmentByIdAsync(Guid departmentId)
        {
            return _repository.Departments
                .Include(p => p.Users)
                .FirstOrDefaultAsync(a => a.Id == departmentId);
        }

        Task<bool> IDepartmentQueries.NameExistsAsync(string name)
        {
            return _repository.Departments
                  .AnyAsync(a => a.Name == name); throw new NotImplementedException();
        }
    }
}
