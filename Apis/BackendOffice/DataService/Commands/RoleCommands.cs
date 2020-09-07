using System.Threading.Tasks;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Commands
{
    public interface IRoleCommands
    {
        Task DeleteRoleAsync(Role role);
    }

    public class RoleCommands : IRoleCommands
    {
        private readonly IBackendOfficeRepository _repository;

        public RoleCommands(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task IRoleCommands.DeleteRoleAsync(Role role)
        {
            _repository.Roles.Remove(role);
            return _repository.SaveChangesAsync();
        }
    }
}
