using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.Queries
{
    public interface ISettingQueries
    {
        IQueryable<Setting> GetSettingQueries();
        Task<Setting> GetSettingByKeyAsync(string key);
    }

    public class SettingQueries : ISettingQueries
    {
        private readonly IBackendOfficeRepository _repository;

        public SettingQueries(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task<Setting> ISettingQueries.GetSettingByKeyAsync(string key)
        {
            return _repository.Settings.FirstOrDefaultAsync(p => p.Key == key);
        }

        IQueryable<Setting> ISettingQueries.GetSettingQueries()
        {
            return _repository.Settings;
        }
    }
}
