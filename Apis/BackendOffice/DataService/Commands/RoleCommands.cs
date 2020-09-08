using System.Threading.Tasks;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Commands
{
    public interface IRoleCommands
    {
        Task CreateRoleAsync(Role role);
        Task DeleteRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
    }

    public class RoleCommands : IRoleCommands
    {
        private readonly IBackendOfficeRepository _repository;

        public RoleCommands(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task IRoleCommands.CreateRoleAsync(Role role)
        {
            _repository.Roles.Add(role);
            return _repository.SaveChangesAsync();
        }

        Task IRoleCommands.DeleteRoleAsync(Role role)
        {
            _repository.Roles.Remove(role);
            return _repository.SaveChangesAsync();
        }

        Task IRoleCommands.UpdateRoleAsync(Role role)
        {
            _repository.Roles.Update(role);
            return _repository.SaveChangesAsync();
        }
    }
}
