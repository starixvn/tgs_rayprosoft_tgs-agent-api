using System.Threading.Tasks;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Commands
{
    public interface IUserCommands
    {
        Task UpdateUserAsync(User user);
    }

    public class UserCommands : IUserCommands
    {
        private readonly IBackendOfficeRepository _repository;

        public UserCommands(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task IUserCommands.UpdateUserAsync(User user)
        {
            _repository.Users.Update(user);
            return _repository.SaveChangesAsync();
        }
    }
}
