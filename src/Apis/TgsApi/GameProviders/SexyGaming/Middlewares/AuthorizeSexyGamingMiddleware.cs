using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.SexyGaming.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SexyGaming.Common;
using TGS.Shared.Game.Models.SexyGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Middlewares
{
    public sealed class AuthorizeSexyGamingMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ISexyGamingServices _services;
        public AuthorizeSexyGamingMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            IJsonSerializationProvider jsonSerializationProvider,
            ISexyGamingServices services)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _services = services ?? throw new ArgumentNullException(nameof(services));
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
            if (data.Action.IgnoreCaseEquals("getBalance") || data.Action.IgnoreCaseEquals("bet") || data.Action.IgnoreCaseEquals("cancelBet"))
            {
                var extension1 = form.GetValue<string>(SexyGamingConsts.Parameters.Extension1);
                var playerInfo = default(SexyGamingGetPlayerInfo);
                if (extension1 != null)
                {
                    var extensionDir = JsonSerializer.Deserialize<Dictionary<string, string>>(extension1);
                    playerInfo = await CheckingTokenAsync(gameProviderId, extensionDir["token"]).ConfigureAwait(false);
                }
                else
                {
                    var message = form.GetValue<string>(SexyGamingConsts.Parameters.Message);
                    var jsonDocument = JsonDocument.Parse(message);
                    JsonElement jsonElement = jsonDocument.RootElement;
                    if (data.Action.IgnoreCaseEquals("getBalance"))
                    {
                        var userIds = _jsonSerializationProvider.DeserializeFromString<SexyGamingUserId>(_jsonSerializationProvider.SerializeToString(jsonDocument));
                        playerInfo = await CheckingUserIdAsync(gameProviderId, userIds.UserId);
                    }
                    else
                    {
                        var userIds = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletTxnsRequest<SexyGamingUserId>>(_jsonSerializationProvider.SerializeToString(jsonDocument));
                        var userIdsGroup = userIds.Transaction.GroupBy(x => x.UserId).Select(x => x.Key).ToArray();
                        if (userIdsGroup.Count() == 1)
                        {
                            playerInfo = await CheckingUserIdAsync(gameProviderId, userIdsGroup.First());
                        }
                        else
                        {
                            throw new MissingTokenException("empty token");
                        }

                    }
                }
                context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, playerInfo.GameProviderConfigurationId);
                context.Items.Add(Consts.ContextItems.PlayerCode, playerInfo.PlayerCode);
                context.Items.Add(Consts.ContextItems.PlatformType, playerInfo.PlatformType);
            }
            await next(context);
        }


        private async Task<SexyGamingGetPlayerInfo> CheckingTokenAsync(Guid gameProviderId, string token)
        {
            var loginRecord = await _services.DecryptTokenAsync(token);

            var playerCode = loginRecord.PlayerCode;
            var platformType = loginRecord.PlatformType;
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            return new SexyGamingGetPlayerInfo()
            {
                GameProviderConfigurationId = bgpc.GameProviderConfigurationId,
                PlayerCode = playerCode,
                PlatformType = platformType
            };
        }

        private async Task<SexyGamingGetPlayerInfo> CheckingUserIdAsync(Guid gameProviderId, string userId)
        {
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(userId);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            return new SexyGamingGetPlayerInfo()
            {
                GameProviderConfigurationId = bgpc.GameProviderConfigurationId,
                PlayerCode = userId,
                PlatformType = PlatformType.Desktop
            };
        }
    }
}