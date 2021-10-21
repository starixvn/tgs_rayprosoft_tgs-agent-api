using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.Evolution;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGameUrl;
using TGS.Shared.Game.Models.Evolution.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public sealed class EvolutionGameInitializer : IGameInitializer
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IEvolutionApiClient _client;
        private readonly IGameProviderCurrencyCodeMapper _currencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlayerOperations _playerOperations;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly IEvolutionService _service;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public EvolutionGameInitializer(
            IBrandGameProviderOperations brandGameProviderOperations,
            IEvolutionApiClient client,
            IPlayerOperations playerOperations,
            IEvolutionService service,
            ISettingOperations settingOperations,
            IGameProviderCurrencyCodeMapper currencyCodeMapper,
            ITokenProvider tokenProvider,
            IGameProviderOperations gameProviderOperations,
            IRequestAnalyzer requestAnalyzer,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _brandGameProviderOperations = brandGameProviderOperations ??
                throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _currencyCodeMapper = currencyCodeMapper ?? throw new ArgumentNullException(nameof(currencyCodeMapper));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));

            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _requestAnalyzer = requestAnalyzer ?? throw new ArgumentNullException(nameof(requestAnalyzer));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var encryptedToken = await _tokenProvider.EncryptAsync(loginTokenId);
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandId = player.BrandId;

            var cultureCode =
                await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, request.Language);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<EvolutionConfiguration>(SettingConsts
                    .GameProviderConfigurations.Evolution);

            var currencyCode =
                await _currencyCodeMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);

            var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId)
                .ConfigureAwait(false);
            var ip = _requestAnalyzer.GetIp(_httpContextAccessor.HttpContext);

            var category = "slots";

            var table = new EvolutionGameUrlConfigGameTable
            {
                Id = cachedGame.ExternalId
            };


            if (cachedGame.Type == GameType.LiveDealerLobby)
            {
                category = "baccarat_sicbo";
                table = null;
            }

            var playerGroup = default(EvolutionGameUrlPlayerGroup);

            var data = new EvolutionGameUrlRequest
            {
                Uuid = GuidExtensions.NewSequentialGuid().ToString(),
                Player = new EvolutionGameUrlPlayer
                {
                    Id = player.PlayerCode,
                    Update = false,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Nickname = player.PlayerCode,
                    Country = "CN",
                    Language = cultureCode,
                    Currency = currencyCode,
                    Session = new EvolutionGameUrlPlayerSession
                    {
                        Id = encryptedToken,
                        Ip = ip
                    },
                    Group = playerGroup
                },
                Config = new EvolutionGameUrlConfig
                {
                    Game = new EvolutionGameUrlConfigGame
                    {
                        Category = category,
                        Table = table
                    },
                    Channel = new EvolutionGameUrlConfigChannel
                    {
                        Wrapped = true,
                        Mobile = token.PlatformType == PlatformType.Mobile
                    }
                }
            };


            var responseGameUrl = await _client.GetGameUrlAsync(configuration, data).ConfigureAwait(false);

            if (responseGameUrl.Errors != null)
            {
                throw new GameProviderApiErrorException(responseGameUrl.Errors[0].Message);
            }

            var path = responseGameUrl.Entry.Substring(1, responseGameUrl.Entry.Length - 1);
            var url = $"{configuration.ApiUrl}{path}";

            return new()
            {
                Url = url
            };
        }
    }
}