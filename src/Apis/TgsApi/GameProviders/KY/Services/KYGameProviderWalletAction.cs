using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.KY.Services
{
    public record KYGameProviderWalletAction : BaseGameProviderWalletAction
    {
        private readonly IKYApiClient _client;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHashProvider _hashProvider;

        public KYGameProviderWalletAction(
            IKYApiClient client,
            IHashProvider hashProvider,
            ITokenProvider tokenProvider,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations
            ) : base(hashProvider, tokenProvider, settingOperations, playerOperations, brandOperations, gameProviderOperations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        public override ValueTask GameProviderFundAsync()
        {
            throw new NotImplementedException();
        }

        public override async ValueTask<decimal?> GameProviderGetBalanceAsync(CachedPlayer player)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                    .GameProviderConfigurations.KY);

            configuration.MerchantCodes.TryGetValue(player.DefaultWalletCurrencyCode, out string agentId);

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();
            var orderId = $"{agentId}{date.ToString("yyyyMMddHHmmss")}{player.PlayerCode}";


            var data = new KYGetBlaanceRequest()
            {
                Action = 1,
                PlayerCode = player.PlayerCode
            };
            var dataPath = KYUtility.GetPath(data);
            var param = KYUtility.AesEncrypt(dataPath, configuration.Deskey, true);
            var plainText = $"{agentId}{timestamp}{configuration.Md5key}";
            var key = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await _client.GetAPIAsync<KYGetBlaanceResponse>(configuration.ApiUrl, apiKYRequest).ConfigureAwait(false);

            var balance = default(decimal);
            if (response.Data.Code == 0)
            {
                balance = response.Data.Balance;
            }

            return balance;
        }
    }
}
