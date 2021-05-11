using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using static TGS.Core.Models.Consts.LocalizationContext;

namespace TGS.Apis.OperatorApi.Handlers.GameProvider
{
    public sealed class GetGameProvidersHandler : IRequestHandler<GetGameProvidersRequest, GetGameProvidersResponse>
    {
        private readonly IBrandCurrencyCodeMapper _brandCurrencyCodeMapper;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly ILocalizationOperations _localizationOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidStatus<Status> _validStatus;

        public GetGameProvidersHandler(
            IHttpContextAccessor httpContextAccessor,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            ILocalizationOperations localizationOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IBrandCurrencyCodeMapper brandCurrencyCodeMapper,
            IValidStatus<Status> validStatus)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _localizationOperations = localizationOperations;
            _brandLanguageCodeMapper = brandLanguageCodeMapper;
            _brandCurrencyCodeMapper = brandCurrencyCodeMapper;
            _validStatus = validStatus;
        }

        async Task<GetGameProvidersResponse> IRequestHandler<GetGameProvidersRequest, GetGameProvidersResponse>.Handle(
            GetGameProvidersRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            var currencyCode =
                await _brandCurrencyCodeMapper.ExternalToInternalAsync(request.CurrencyCode, brandId);

            var playerTokenData = _httpContext.Items.GetPlayerTokenData();
            var platformType = request.PlatformType;

            CachedPlayer cachedPlayer = null;

            if (playerTokenData != null)
            {
                if (playerTokenData.PlatformType != platformType)
                {
                    throw new InvalidOperationException("request platform is not matched with player login platform.");
                }

                cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode);
                if (cachedPlayer != null && cachedPlayer.DefaultWalletCurrencyCode.IgnoreCaseNotEquals(currencyCode))
                {
                    throw new CurrencyNotMatchingException(cachedPlayer.DefaultWalletCurrencyCode, currencyCode);
                }
            }

            var playerType = cachedPlayer?.Type ?? PlayerType.RealPlayer;
            var culture = await _brandLanguageCodeMapper.ExternalToInternalAsync(request.LanguageCode, brandId);
            var providers = await GetGameProviderModelsAsync(brandId, currencyCode, platformType, playerType);

            var translatedProviders = new List<GameProviderModel>();
            foreach (var gpm in providers)
            {
                var key = $"{gpm.Id}";
                var store = await _localizationOperations.GetLocalizationByGameProviderIdAsync(gpm.Id, culture);

                var model = gpm with
                {
                    Name = _localizationOperations.Parse(store, key, GameProviderDescription, gpm.Name),
                    Description =
                    _localizationOperations.Parse(store, key, GameProviderDescription, gpm.Description)
                };

                translatedProviders.Add(model);
            }

            return new() {GameProviders = translatedProviders.ToArray()};
        }

        private async Task<GameProviderModel[]> GetGameProviderModelsAsync(Guid brandId,
            string currencyCode, PlatformType platformType,
            PlayerType playerType)
        {
            var gameProviders = await _brandOperations.GetBrandGameProvidersAsync(brandId);
            var statuses = _validStatus.GetStatus(playerType);

            var providers = new List<GameProviderModel>();

            foreach (var bgp in gameProviders.Where(bgp => statuses.Contains(bgp.BrandGameProviderStatus)))
            {
                var gameProviderId = bgp.GameProviderId;
                var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(bgp.GameProviderId);
                var currencySupported =
                    await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, currencyCode);

                if (!currencySupported)
                {
                    continue;
                }

                var gamePlatformTypes = platformType.ToGamePlatformTypes();
                var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, gamePlatformTypes);

                if (games.Length == 0)
                {
                    continue;
                }

                var model = new GameProviderModel
                {
                    Id = gameProviderId,
                    Code = gameProvider.Code,
                    Name = gameProvider.Name,
                    Description = gameProvider.Description,
                    Status = gameProvider.Status,
                    TransferWallet = gameProvider.TransferWallet,
                    GameTypes = games.Select(g => g.Type).OrderBy(t => (int)t)
                        .Distinct().ToArray()
                };

                providers.Add(model);
            }

            return providers.Where(gpm => gpm != null).ToArray();
        }
    }
}