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
using TGS.Shared.Game.Models.KY.Incoming;
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
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            var brandId = player.BrandId;
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);


            var gpc = await _gameProviderOperations.GetCachedGameProviderConfigurationAsync(gameProviderId,
                bgpc.GameProviderConfigurationId);

            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId);

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
            var dataPath = GetPath(data);
            var param = AesEncrypt(dataPath, configuration.Deskey, true);
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

        private string GetPath<TRequest>(TRequest request)
        {
            var companies = JObject.Parse(JsonSerializer.Serialize(request)).Properties();
            var keyValuePairs = companies.ToDictionary(k => k.Name, v => v.Value.ToString());
            return string.Join("&", keyValuePairs.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">需加密字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="isEncodeUrl">是否需要URL编码</param>
        /// <returns></returns>
        private string AesEncrypt(string str, string key, bool isEncodeUrl = true)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            var base64Str = Convert.ToBase64String(resultArray, 0, resultArray.Length);

            return isEncodeUrl ? System.Web.HttpUtility.UrlEncode(base64Str) : base64Str;
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">需解密字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="isDecodeUrl">是否需要URL解码</param>
        /// <returns></returns>
        private string AesDecrypt(string str, string key, bool isDecodeUrl = true)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (isDecodeUrl)
                str = System.Web.HttpUtility.UrlDecode(str);

            Byte[] toEncryptArray = Convert.FromBase64String(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
