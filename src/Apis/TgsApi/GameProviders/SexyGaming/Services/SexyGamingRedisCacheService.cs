using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Models.Services.Data;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public class SexyGamingRedisCacheService : ISexyGamingRedisCacheService
    {
        private readonly IDatabase _database;
        private readonly string _keyBrand = "sexygaming:brand:{0}";
        private readonly string _keyCancelBetCache = "sexygaming:cancelbet:{0}";
        private readonly string _keyGameActionExternal = "sexygaming:gameactionexternal:{0}:{1}:{2}";
        private readonly string _keyGameProviderConfiguration = "sexygaming:gameproviderconfiguration:{0}:{1}";
        private readonly ISexyGamingServices _services;

        public SexyGamingRedisCacheService(
            IDatabase database,
            ISexyGamingServices services
            )
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public async ValueTask AddCancelBetCacheAsync(SexyGamingCancelBet[] source)
        {
            var platformTxIdJoin = string.Join('|', source.Select(x => x.PlatformTxId).ToArray());
            var key = string.Format(_keyCancelBetCache, platformTxIdJoin);
            await _database.StringSetAsync(key, platformTxIdJoin, TimeSpan.FromDays(1)).ConfigureAwait(false);
        }

        public async ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode)
        {
            var key = string.Format(_keyBrand, playerCode);
            if (await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                return Guid.Parse(await _database.StringGetAsync(key).ConfigureAwait(false));
            }
            var brandId = await _services.GetBrandIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);

            await _database.StringSetAsync(key, brandId.ToString(), TimeSpan.FromDays(1)).ConfigureAwait(false);
            return brandId;
        }

        public async ValueTask<string> GetCancelBetPlatformTxIdJoinAsync(SexyGamingBet[] source)
        {
            var platformTxIdJoin = string.Join('|', source.Select(x => x.PlatformTxId).ToArray());
            var key = string.Format(_keyCancelBetCache, platformTxIdJoin);
            return await _database.StringGetAsync(key).ConfigureAwait(false);
        }

        public async ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
        {
            var key = string.Format(_keyGameActionExternal, gameProviderId, playerCode, externalTransactionId);
            if (await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                return JsonSerializer.Deserialize<GameActionExternalIds>(await _database.StringGetAsync(key).ConfigureAwait(false));
            }
            var gameAction = await _services.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);
            if (gameAction != null)
            {
                await _database.StringSetAsync(key, JsonSerializer.Serialize(gameAction), TimeSpan.FromDays(1)).ConfigureAwait(false);
                return gameAction;
            }
            return null;
        }

        public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
        {
            var key = string.Format(_keyGameProviderConfiguration, brandId, gameProviderId);
            if (await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                return Guid.Parse(await _database.StringGetAsync(key).ConfigureAwait(false));
            }
            var gameProviderConfigurationId = await _services.GetGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);

            await _database.StringSetAsync(key, gameProviderConfigurationId.ToString(), TimeSpan.FromDays(1)).ConfigureAwait(false);
            return gameProviderConfigurationId;
        }
    }
}
