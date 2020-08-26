using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Queries
{
    public interface IUserQueries
    {
        Task<User> GetUserByUsernameAsync(string username);
    }

    public class UserQueries : IUserQueries
    {
        private readonly IBackendOfficeRepository _repository;

        public UserQueries(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task<User> IUserQueries.GetUserByUsernameAsync(string username)
        {
            return _repository.Users
                .Include(u => u.Role)
                .Include(u => u.Role.Menus)
                .ThenInclude(p => p.Menu)
                .Include(u => u.Role.Menus)
                .ThenInclude(p => p.SubMenus)
                .ThenInclude(p => p.Permissions)
                .ThenInclude(p => p.Permission)
                .FirstOrDefaultAsync(a => a.Username == username && a.Status == UserStatus.Enabled);
        }
    }
}
