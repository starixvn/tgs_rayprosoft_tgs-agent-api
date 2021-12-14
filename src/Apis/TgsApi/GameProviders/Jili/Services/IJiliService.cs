using System;
using System.Threading.Tasks;
using TGS.Core.Models.GameProviders;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public interface IJiliService
    {
        ValueTask<JiliConfiguration> GetConfigurationAsync();

        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();

        ValueTask<Guid> GetGameProviderIdAsync();
    }
}
