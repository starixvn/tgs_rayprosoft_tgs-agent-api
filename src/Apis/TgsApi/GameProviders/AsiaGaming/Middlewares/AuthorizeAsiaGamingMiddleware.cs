using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.AsiaGaming.Enums;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Middlewares
{
    public sealed class AuthorizeAsiaGamingMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public AuthorizeAsiaGamingMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations)
        {
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _settingOperations = settingOperations;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = await context.Request.ReadAsStringAsync();
            var elements = GetElements(value);

            var betResponse = GetElementValue(elements, "betResponse").IgnoreCaseEquals("OK");
            var agentCode = await GetAgentCodeAsync(elements, betResponse);

            // playername={agentCode}{playerCode}
            var playerCode = GetPlayerCode(agentCode, GetElementValue(elements, "playname"));

            var transactionType = GetTransactionType(elements, betResponse);

            var tokenId = GetElementValue(elements, "sessionToken");
            var loginRecord = await DecryptTokenAsync(tokenId, transactionType);

            var platformType = loginRecord.PlatformType;
            var gameProviderId =
                await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.AsiaGaming);
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);

            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
            var gameProviderConfigurationId = bgpc.GameProviderConfigurationId;

            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, gameProviderConfigurationId);
            context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
            context.Items.Add(Consts.ContextItems.PlatformType, platformType);

            await next(context);
        }

        private async Task<CachedPlayerLoginRecord> DecryptTokenAsync(string tokenId, TransactionType transactionType)
        {
            if (tokenId.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = Guid.Parse(tokenId);
            var loginRecord = await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);
            var isPlaceBet = transactionType == TransactionType.BET;

            if (isPlaceBet && loginRecord == null)
            {
                throw new PlayerLoginExpiredException();
            }

            return loginRecord;
        }

        private async Task<string> GetAgentCodeAsync(IReadOnlyDictionary<string, string> elements, bool betResponse)
        {
            if (!betResponse)
            {
                return GetElementValue(elements, "agentCode");
            }

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.AsiaGaming);

            return configuration.ProductionId;
        }

        private TransactionType GetTransactionType(IReadOnlyDictionary<string, string> elements, bool betResponse)
        {
            if (betResponse)
            {
                return TransactionType.BET;
            }

            Enum.TryParse<TransactionType>(GetElementValue(elements, "transactionType"), true, out var transactionType);
            return transactionType;
        }

        private static IReadOnlyDictionary<string, string> GetElements(string xml)
        {
            var xDoc = XDocument.Parse(xml);

            if (xDoc.Root != null && xDoc.Root.Elements().Any(e => e.Name.LocalName.Contains("betResponse")))
            {
                return xDoc.Root.Elements().Where(e => !e.Name.LocalName.Contains("record"))
                    .ToDictionary(a => a.Name.LocalName, a => a.Value);
            }

            return xDoc.Root?.Elements().First(e => e.Name.LocalName.Contains("Record"))
                    .Elements().Where(e => !e.Name.LocalName.Contains("transactionID"))
                    .ToDictionary(a => a.Name.LocalName, a => a.Value) ??
                new Dictionary<string, string>();
        }

        private static string GetElementValue(IReadOnlyDictionary<string, string> elements, string name) =>
            elements.TryGetValue(name, out var value) ? value : string.Empty;

        private static string GetPlayerCode(string agentCode, string playerName) =>
            playerName.IsNullOrEmpty() ? playerName : playerName.Replace(agentCode, "");
    }
}