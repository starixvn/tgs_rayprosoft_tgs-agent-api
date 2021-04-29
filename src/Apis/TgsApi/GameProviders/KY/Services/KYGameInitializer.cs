using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.KY.Services
{
    public record KYGameInitializer : IGameInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKYApiClient _client;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHashProvider _hashProvider;
        private readonly IRequestAnalyzer _requestAnalyzer;

        public KYGameInitializer(
            IHttpContextAccessor httpContextAccessor,
            IKYApiClient client,
            IHashProvider hashProvider,
            ITokenProvider tokenProvider,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IRequestAnalyzer requestAnalyzer
            )
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _requestAnalyzer = requestAnalyzer ?? throw new ArgumentNullException(nameof(requestAnalyzer));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var encryptedToken = await _tokenProvider.EncryptAsync(loginTokenId);

            var player = await _playerOperations.GetCachedPlayerAsync(token.PlayerCode);


            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                    .GameProviderConfigurations.KY);

            configuration.MerchantCodes.TryGetValue(player.DefaultWalletCurrencyCode, out string agentId);

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();
            var orderId = $"{agentId}{date.ToString("yyyyMMddHHmmss")}{token.PlayerCode}";
            var ip = _requestAnalyzer.GetIp(_httpContextAccessor.HttpContext);

            var data = new KYGameUrlRequest()
            {
                Action = 0,
                PlayerCode = token.PlayerCode,
                Orderid = orderId,
                Amount = 0m,
                IP = ip,
                ExternalID = 0,

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

            var response = await _client.GetAPIAsync<KYGameUrlResponse>(configuration.ApiUrl, apiKYRequest).ConfigureAwait(false);

            string url = (string)null;
            if (response.Data.Code == 0)
            {
                url = response.Data.Url;
            }

            return new() { Url = url };
        }
    }
}
