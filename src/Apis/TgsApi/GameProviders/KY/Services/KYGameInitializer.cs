using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.KY.Services
{
    public record KYGameInitializer : IGameInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKYApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IHashProvider _hashProvider;
        private readonly IRequestAnalyzer _requestAnalyzer;

        public KYGameInitializer(
            IHttpContextAccessor httpContextAccessor,
            IKYApiClient client,
            IHashProvider hashProvider,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IRequestAnalyzer requestAnalyzer
            )
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _requestAnalyzer = requestAnalyzer ?? throw new ArgumentNullException(nameof(requestAnalyzer));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {

            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                    .GameProviderConfigurations.KY);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status).FirstOrDefault()?.AgentId;

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();
            var orderId = $"{agentId}{date.ToString("yyyyMMddHHmmss")}{player.PlayerCode}";
            var ip = _requestAnalyzer.GetIp(_httpContextAccessor.HttpContext);

            var data = new KYGameUrlRequest()
            {
                Action = KYUtility.GetAction(GPAction.GetGameUrl) ?? -1,
                PlayerCode = player.PlayerCode,
                OrderId = orderId,
                Amount = 0m,
                IP = ip,
                BrandsCode = player.BrandCode,
                ExternalID = 0,

            };
            var dataPath = KYUtility.GetPath(data);
            var param = KYUtility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await _client.GetAPIAsync<KYGameUrlResponse>(brandMerchant.ApiUrl, apiKYRequest).ConfigureAwait(false);

            string url = (string)null;
            if (response.Data.Code == 0)
            {
                url = response.Data.Url;
            }

            return new() { Url = url };
        }
    }
}
