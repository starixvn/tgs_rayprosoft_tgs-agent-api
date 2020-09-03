using System.Threading.Tasks;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Commands
{
    public interface ISettingCommands
    {
        Task UpdateSettingAsync(Setting setting);
        Task DeleteSettingAsync(Setting setting);
    }

    public class SettingCommands : ISettingCommands
    {
        private readonly IBackendOfficeRepository _repository;

        public SettingCommands(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task ISettingCommands.DeleteSettingAsync(Setting setting)
        {
            _repository.Settings.Remove(setting);
            return _repository.SaveChangesAsync();
        }

        Task ISettingCommands.UpdateSettingAsync(Setting setting)
        {
            _repository.Settings.Update(setting);
            return _repository.SaveChangesAsync();
        }
    }
}
