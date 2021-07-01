using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using TGS.GameProviders.Repositories.Models;

namespace TGS.GameProviders.Repositories
{
    public interface IGameProviderScheduleRepository
    {
        ValueTask<GameProviderSchedule> GetAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode);

        ValueTask<UpdateResult> MergeAsync(GameProviderSchedule data);
    }
}
