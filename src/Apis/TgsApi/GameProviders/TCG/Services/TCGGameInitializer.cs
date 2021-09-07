using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings.TCG;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.TCG;
using TGS.Shared.Game.Models.TCG.Enums;
using TGS.Shared.Game.Models.TCG.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public sealed class TCGGameInitializer : IGameInitializer
    {
        private readonly ITCGApiClient _client;
        private readonly ITCGService _service;
        private readonly IJsonSerializationProvider _json;

        private readonly IReadOnlyDictionary<string, Func<Uri, TCGMerchant, TCGLaunchGameRequest, bool, ValueTask<TCGLaunchGameResponse>>> _methodMapper;

        public TCGGameInitializer(
            ITCGApiClient client,
            ITCGService service,
            IJsonSerializationProvider json
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _methodMapper = new Dictionary<string, Func<Uri, TCGMerchant, TCGLaunchGameRequest, bool, ValueTask<TCGLaunchGameResponse>>>
            {
                { "TCG_SEA",SEAAsync },
                { "TCG LOTTO",ELOTTOAsync },
                { "TCG_LOTTO_VN",LOTTOVNAsync }
            };
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _service.GetCachedPlayerAsync(request.Token.PlayerCode).ConfigureAwait(false);
            var gameProviderId = await _service.GetGameProviderIdAsync();
            var configuration = await _service.GetConfigurationAsync();
            var cultureCode = await _service.GetGameProviderCultureCodeAsync(gameProviderId, player.BrandId, request.Language);
            var currencyCode = await _service.GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode);
            var merchant = await _service.GetMerchantAsync(player.DefaultWalletCurrencyCode);
            var cachedGame = await _service.GetCachedGameAsync(gameProviderId, request.GameId);


            if (!_methodMapper.TryGetValue(cachedGame.ExternalId.ToUpper(), out var execute))
            {
                throw new GameProviderApiErrorException($"GameId{request.GameId} could not find the corresponding lottery entry (GameId{request.GameId}找不到對應彩票入口)");
            }
            var data = new TCGLaunchGameRequest()
            {
                UserName = request.Token.PlayerCode,
                Language = cultureCode

            };

            var response = await execute(configuration.ApiUrl, merchant, data, request.Token.PlatformType == PlatformType.Mobile).ConfigureAwait(false);

            if (response.Status == 15)
            {
                var playerResponse = await _client.RegisterPlayerAsync(configuration.ApiUrl, merchant, new TCGRegisterPlayerRequest()
                {
                    UserName = request.Token.PlayerCode,
                    Password = merchant.PlayerPassword
                }).ConfigureAwait(false);
                if (playerResponse.Status != 0)
                {
                    throw new GameProviderApiErrorException(response.ErrorDesc);
                }
                response = await execute(configuration.ApiUrl, merchant, data, request.Token.PlatformType == PlatformType.Mobile).ConfigureAwait(false);
            }
            else
            {
                if (response.Status != 0)
                {
                    throw new GameProviderApiErrorException(response.ErrorDesc);
                }
            }
            return new() { Url = response.GameUrl };
        }
        private ValueTask<TCGLaunchGameResponse> ELOTTOAsync(Uri apiUrl, TCGMerchant merchant, TCGLaunchGameRequest source, bool IsMobile = false)
        {

            var data = new TCGLaunchGameByELOTTORequest()
            {
                UserName = source.UserName,
                Platform = "html5",
                GameMode = ((int)TCGPlayerGameMode.RealPlayer).ToString(),
                GameCode = "Lobby",
                Language = source.Language,
                LotteryBetMode = "Elott_Traditional",
                View = "Lobby"
            };
            if (IsMobile)
            {
                data.GameCode = "game_list";
                data.LotteryBetMode = "Elott_Traditional_Mobile_V2";
                data.View = "game_list";
            }
            return _client.LaunchGameAsync(apiUrl, merchant, data);
        }
        private ValueTask<TCGLaunchGameResponse> SEAAsync(Uri apiUrl, TCGMerchant merchant, TCGLaunchGameRequest source, bool IsMobile = false)
        {
            var data = new TCGLaunchGameBySEARequest()
            {
                UserName = source.UserName,
                Platform = "html5",
                GameMode = ((int)TCGPlayerGameMode.RealPlayer).ToString(),
                GameCode = "lobby",
                Language = source.Language,
                LotteryBetMode = "SEA_Tradition",
                View = "lobby"
            };
            return _client.LaunchGameAsync(apiUrl, merchant, data);
        }
        private ValueTask<TCGLaunchGameResponse> LOTTOVNAsync(Uri apiUrl, TCGMerchant merchant, TCGLaunchGameRequest source, bool IsMobile = false)
        {
            var data = new TCGLaunchGameByLOTTOVNRequest()
            {
                UserName = source.UserName,
                Platform = "WEB",
                GameMode = ((int)TCGPlayerGameMode.RealPlayer).ToString(),
                GameCode = "lobby",
                Language = source.Language,
                LotteryBetMode = "Elott",
                View = "lobby"
            };

            return _client.LaunchGameAsync(apiUrl, merchant, data);
        }
    }
}
