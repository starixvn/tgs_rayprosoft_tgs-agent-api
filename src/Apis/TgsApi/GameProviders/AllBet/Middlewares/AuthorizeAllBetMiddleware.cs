using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.AllBet.Exceptions;
using TGS.Apis.TgsApi.GameProviders.AllBet.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.AllBet.Enums;
using TGS.Shared.Game.Models.AllBet.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Middlewares
{
    public sealed class AuthorizeAllBetMiddleware : IMiddleware
    {
        private static readonly string _Routing = "/api/ab";

        private readonly IBrandOperations _brandOperations;
        private readonly IAllBetCacheServices _cacheServices;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IAllBetPlayerServices _services;
        private readonly ISettingOperations _settingOperations;

        public AuthorizeAllBetMiddleware(
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandOperations brandOperations,
            IPlayerOperations playerOperations,
            IAllBetPlayerServices services,
            IAllBetCacheServices cacheServices)
        {
            _settingOperations = settingOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandOperations = brandOperations;
            _playerOperations = playerOperations;
            _services = services;
            _cacheServices = cacheServices;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var headers = context.Request.Headers;
            var (propertyId, signature) = GetPropertyIdAndSignature(headers);

            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<AllBetConfiguration>(
                SettingConsts.GameProviderConfigurations.AllBet);

            var agent = configuration.BrandMerchants.SelectMany(m => m.Agents)
                .FirstOrDefault(a => a.PropertyId.Equals(propertyId));

            if (agent == null)
            {
                throw new InvalidOperatorIdException($"No agent of property id '{propertyId}' is found.");
            }

            var path = GetEndPath(context.Request.Path);
            var bypassSignatureVerify = bool.Parse(GetHeaderValue(headers, "BypassSignatureVerify", bool.FalseString));

            if (!bypassSignatureVerify)
            {
                var computedSignature = ComputeSignature(headers, context.Request.Method, path, agent.PartnerApiKey);

                if (signature.NotEquals(computedSignature))
                {
                    throw new InvalidCredentialsException(
                        $"Signature is mismatched. The signature in the request is {signature}, which is different from {computedSignature} as computed.");
                }
            }

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.AllBet);

            context.Items.Add(AllBetConsts.ContextItems.Agent, agent);
            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

            if (path.Contains("CancelTransfer"))
            {
                var request = await context.Request.ReadAsObjectFromJsonAsync<CancelTransferRequest>();
                var externalGameActionGroupId = request.ReferencedTransactionId.ToString();
                var playerCode =
                    await _cacheServices.GetPlayerCodeByExternalGameActionGroupIdAsync(externalGameActionGroupId);

                if (playerCode.IsNullOrEmpty())
                {
                    throw new InvalidGameActionException($"Player by tranId '{externalGameActionGroupId}' is not found",
                        InvalidGameActionErrorCode.TransactionNotFound);
                }

                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
                var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
                var gameProviderConfigurationId = bgpc.GameProviderConfigurationId;

                context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
                context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, gameProviderConfigurationId);
            }
            else if (path.Contains("Transfer"))
            {
                var request = await context.Request.ReadAsObjectFromJsonAsync<TransferRequest>();
                var playerCode = _services.GetPlayerCode(request.PlayerId, agent.Suffix);
                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
                var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
                var gameProviderConfigurationId = bgpc.GameProviderConfigurationId;

                context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
                context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, gameProviderConfigurationId);

                if (request.Type == TransferType.PlaceBet)
                {
                    var platformType = request.Details.Any(x => x.AppType == 3)
                        ? PlatformType.Mobile
                        : PlatformType.Desktop;

                    context.Items.Add(Consts.ContextItems.PlatformType, platformType);
                }
            }

            await next(context);
        }

        private static (string PropertyId, string Signature) GetPropertyIdAndSignature(IHeaderDictionary headers)
        {
            var authorization = GetHeaderValue(headers, "Authorization", string.Empty);

            if (authorization.IsNullOrEmpty())
            {
                throw new InvalidCredentialsException("Authorization header is empty");
            }

            var items = authorization.Replace("AB ", string.Empty).Split(":");
            if (items.Length != 2)
            {
                throw new InvalidCredentialsException("Invalid signature format");
            }

            return (items[0], items[1]);
        }

        private static string ComputeSignature(IHeaderDictionary headers, string method, string path,
            string partnerApiKey)
        {
            var md5 = GetHeaderValue(headers, "Content-MD5", string.Empty);
            var contentType = GetHeaderValue(headers, "Content-Type", string.Empty);
            var date = GetHeaderValue(headers, "Date", string.Empty);

            var stringToSign = string.Join("\n", method, md5, contentType, date, path);
            using var hasher = new HMACSHA1(Convert.FromBase64String(partnerApiKey));
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            return hash.ToBase64String();
        }

        private static string GetEndPath(PathString pathString)
        {
            var path = pathString.ToUriComponent();
            var idx = path.IndexOf(_Routing, StringComparison.OrdinalIgnoreCase);
            return path[idx..].Replace(_Routing, string.Empty);
        }

        private static string GetHeaderValue(IHeaderDictionary headers, string key, string defaultValue)
            => headers.TryGetValue(key, out var value) ? value.ToString() : defaultValue;
    }
}