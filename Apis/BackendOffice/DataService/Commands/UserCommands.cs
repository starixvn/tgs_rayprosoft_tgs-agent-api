using System.Threading.Tasks;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Commands
{
    public interface IUserCommands
    {
        Task UpdateUserAsync(User user);
        Task CreateUserAsync(User user);
    }

    public class UserCommands : IUserCommands
    {
        private readonly IBackendOfficeRepository _repository;

        public UserCommands(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        async Task IUserCommands.CreateUserAsync(User user)
        {
            await _repository.Users.AddAsync(user);
            await _repository.SaveChangesAsync();
        }

        Task IUserCommands.UpdateUserAsync(User user)
        {
            _repository.Users.Update(user);
            return _repository.SaveChangesAsync();
        }
    }
}
