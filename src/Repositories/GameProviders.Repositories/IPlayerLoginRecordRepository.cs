using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Models;

namespace TGS.GameProviders.Repositories
{
    public interface IPlayerLoginRecordRepository
    {
        Task<PlayerLoginRecordModel> GetAsync(Guid loginRecordId);

        Task<PlayerLoginRecordModel[]> QueryEnabledAsync(string playerCode);
    }
}
