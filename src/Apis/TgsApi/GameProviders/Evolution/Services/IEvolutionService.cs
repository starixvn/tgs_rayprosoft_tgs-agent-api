using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.GameProviders.Repositories.Models;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services;

public interface IEvolutionService
{
    ValueTask<PlayerLoginRecordModel> DecryptTokenAsync(string token);

    (bool RoundClosed, string ExternalTransactionId) GetCancelExternalTransactionId(
        string externalTransactionId,
        GameActionType gameActionType);

    ValueTask<EvolutionConfiguration> GetConfigurationAsync();

    ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsAsync(
         Guid gameProviderId,
         string playerCode,
         string externalRoundId,
         int gameId,
         GameActionType[] gameActionTypes);

    ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsByExternalGameActionGroupIdAsync(
        Guid gameProviderId,
        string playerCode,
        string externalGameActionGroupId);

    ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
        Guid gameProviderId,
        Guid gameProviderConfigurationId,
        PlatformType platformType);

    ValueTask<Guid> GetGameProviderIdAsync();

    void IgnoreCaseNotEqualsUserId(string userId, string playerCode);

    ValueTask<bool> NotExistBetGameActionExternalIdAsync(
        Guid gameProviderId,
        string playerCode,
        string externalRoundId,
        string externalGameId,
        string externalGameActionGroupId);
}
