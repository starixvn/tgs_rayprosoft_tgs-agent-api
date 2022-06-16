using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings.JDB;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services;

public interface IJDBServices
{
    JDBAgent GetAgent(IEnumerable<JDBAgent> agents, string currencyCode);

    ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(PlatformType platformType);
}