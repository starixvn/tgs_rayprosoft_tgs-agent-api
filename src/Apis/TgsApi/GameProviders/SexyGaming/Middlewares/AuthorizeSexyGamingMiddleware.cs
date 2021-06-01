using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SexyGaming.Common;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Middlewares
{
    public sealed class AuthorizeSexyGamingMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        public AuthorizeSexyGamingMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            ITokenProvider tokenProvider,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SexyGamingConfiguration>(SettingConsts.GameProviderConfigurations.SexyGaming);

            var form = await context.Request.ReadFormAsync();
            var key = form.GetValue<string>(SexyGamingConsts.Parameters.Key);

            if (key.IgnoreCaseNotEquals(configuration.Cert))
            {
                throw new MissingTokenException($"Authentication encryption failed({key})");
            }

            var Message = JsonDocument.Parse(form.GetValue<string>(SexyGamingConsts.Parameters.Message));
            var data = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletBaseRequest>(
                _jsonSerializationProvider.SerializeToString(Message));

            var gameProviderId =
                    await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.SexyGaming);
            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

            /*
             * Place bet: 1 player / multiple transactions (1 confirm action)
             * Cancel bet: 1 player / multiple transactions (1 confirm action)
             * Settle: multiple player / multiple transactions (1 round action)
             * Unsettle: multiple player / multiple transactions (1 round action)
             * Voidbet: multiple player / multiple transactions (1 round action)
             * VoidSettle: 1 player / multiple transaction (by selected transaction)
             */
            /*
             * 貴司應該會遇到這三個流程
             * Unsettle : 當出現技術問題需要更換成績時。 流程 : bet -> settle -> 技術問題需要改成績 -> unsettle  -> settle
             * Void Settle : 當出現技術問題要將使用者單筆交易void時。 流程 : bet -> settle -> 技術問題需要void使用者單筆交易 -> void settle
             * Void bet : 當出現技術問題要將整局void時。 流程 : bet -> settle -> 技術問題需要void整局 -> unsettle -> void bet
             */
            if (data.Action.IgnoreCaseEquals("getBalance") || data.Action.IgnoreCaseEquals("bet") || data.Action.IgnoreCaseEquals("cancelBet") || data.Action.IgnoreCaseEquals("voidSettle"))
            {
                var extension1 = form.GetValue<string>(SexyGamingConsts.Parameters.Extension1);
                var extensionDir = JsonSerializer.Deserialize<Dictionary<string, string>>(extension1);
                var loginRecord = await DecryptTokenAsync(extensionDir["token"]);

                var playerCode = loginRecord.PlayerCode;
                var platformType = loginRecord.PlatformType;
                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
                var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);


                context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
                context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
                context.Items.Add(Consts.ContextItems.PlatformType, platformType);

            }
            await next(context);
        }

        private async Task<CachedPlayerLoginRecord> DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);
            return await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);
        }
    }
}
