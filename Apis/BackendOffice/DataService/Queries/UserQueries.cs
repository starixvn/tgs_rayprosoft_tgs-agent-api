using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Queries
{
    public interface IUserQueries
    {
        Task<User> GetUserByUsernameAsync(string username);

        IQueryable<User> GetUseQueries();
    }

    public class UserQueries : IUserQueries
    {
        private readonly IBackendOfficeRepository _repository;

        public UserQueries(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        IQueryable<User> IUserQueries.GetUseQueries()
        {
            return _repository.Users.Where(p => p.Status == UserStatus.Archived);
        }

        Task<User> IUserQueries.GetUserByUsernameAsync(string username)
        {
            return _repository.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(a => a.Username == username && a.Status == UserStatus.Enabled);
        }
    }
}
