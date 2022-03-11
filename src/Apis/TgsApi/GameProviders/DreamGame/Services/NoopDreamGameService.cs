using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.GameProviders.Repositories.Models;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Services
{
    internal class NoopDreamGameService : IDreamGameService
    {
        public ValueTask<bool> ExistGameActionExternalIdAsync(Guid gameProviderId, string externalTransactionId) => throw new NotImplementedException();
        public ValueTask<bool> GetAutoCloseRoundAsync() => throw new NotImplementedException();
        public ValueTask<DreamGameConfiguration> GetDreamGameConfigurationAsync() => throw new NotImplementedException();
        public ValueTask<GameActionExternalIds> GetGameActionExternalIdByCancelTransactionAsync(Guid gameProviderId, string playerCode, string externalTransactionId) => throw new NotImplementedException();
        public ValueTask<GameActionExternalIds> GetGameActionExternalIdByPlaceBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId) => throw new NotImplementedException();
        public ValueTask<GameActionExternalIds> GetGameActionExternalIdBySettleAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId) => throw new NotImplementedException();
        public ValueTask<OrderInfo[]> GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId) => throw new NotImplementedException();
        public ValueTask<CachedGame> GetGameAsync() => throw new NotImplementedException();
        public ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId) => throw new NotImplementedException();
        public ValueTask<Guid> GetGameProviderIdAsync() => throw new NotImplementedException();
        public string GetHashMD5(string agentName, string ApiKey) => throw new NotImplementedException();
        public ValueTask<PlayerLoginRecordModel> GetPlayerLoginLastAsync(string playerCode) => throw new NotImplementedException();
        public ValueTask<OrderInfo[]> GetUnsettleAsync(Guid gameProviderId) => throw new NotImplementedException();
    }
}
