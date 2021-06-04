// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.SBOBET;
using TGS.Shared.Game.Models.SBOBET.Outgoing;
using static TGS.Shared.Game.Models.SBOBET.Enums.ServiceApiError;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public sealed class SBOBETPlayerAction : IPlayerAction
    {
        private readonly IBetLimitQueries _betLimitQueries;
        private readonly ISBOApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public SBOBETPlayerAction(ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBetLimitQueries betLimitQueries,
            ISBOApiClient client)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _betLimitQueries = betLimitQueries;
            _client = client;
        }

        async ValueTask<PlayerKickResponse> IPlayerAction.KickAsync(PlayerKickRequest request)
        {
            var configuration =
                await _settingOperations
                    .GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts.GameProviderConfigurations.SBOBET)
                    .ConfigureAwait(false);

            var response = await _client.LogoutAsync(configuration.ApiUrl,
                new()
                {
                    CompanyKey = configuration.CompanyKey,
                    PlayerCode = request.PlayerCode
                }).ConfigureAwait(false);

            if (response.Error.Code != None)
            {
                throw new GameProviderApiErrorException(response.Error.Message);
            }

            return new();
        }

        async ValueTask<PlayerUpdateSportBetLimitsResponse> IPlayerAction.UpdateSportBetLimitsAsync(
            PlayerUpdateSportBetLimitsRequest request)
        {
            var configuration =
                await _settingOperations
                    .GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts.GameProviderConfigurations.SBOBET)
                    .ConfigureAwait(false);

            var playerCode = request.PlayerCode;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode)
                .ConfigureAwait(false);

            var betLimits =
                await _betLimitQueries.GetPlayerSportBetLimitsAsync(playerCode, gameProviderId,
                    request.SportMarketTypeIds).ConfigureAwait(false);

            var response = await UpdatePlayerBetLimitsAsync(playerCode, betLimits, configuration)
                .ConfigureAwait(false);

            if (response.Error.Code != None)
            {
                throw new GameProviderApiErrorException(response.Error.Message);
            }

            return new();
        }

        private async ValueTask<BaseResponse> UpdatePlayerBetLimitsAsync(string playerCode,
            SportBetLimitResult[] betLimits,
            SBOConfiguration configuration)
        {
            var apiUrl = configuration.ApiUrl;
            var companyKey = configuration.CompanyKey;

            var updateBetLimitRequest = new UpdatePlayerBetLimitsRequest
            {
                CompanyKey = companyKey,
                PlayerCode = playerCode,
                BetLimits = betLimits.Select(bl => new UpdatePlayerBetLimitsRequest.BetLimit
                {
                    SportType = int.Parse(bl.SportTypeExternalId),
                    MarketType = int.Parse(bl.MarketTypeExternalId),
                    MinBet = bl.MinBet,
                    MaxBet = bl.MaxBet,
                    MaxBetPerMatch = bl.MaxBetPerMatch
                }).ToArray()
            };

            var response = await _client.UpdatePlayerBetLimitsAsync(apiUrl, updateBetLimitRequest)
                .ConfigureAwait(false);

            if (response.Error.Code == None || response.Error.Code != UserNotExist)
            {
                return response;
            }

            var player = await _playerOperations.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);

            var registerResponse = await _client.RegisterPlayerAsync(apiUrl,
                new(configuration.AgentPrefix, player.DefaultWalletCurrencyCode)
                {
                    CompanyKey = companyKey,
                    PlayerCode = playerCode
                }).ConfigureAwait(false);

            if (registerResponse.Error.Code != None)
            {
                return registerResponse;
            }

            return await _client.UpdatePlayerBetLimitsAsync(apiUrl, updateBetLimitRequest).ConfigureAwait(false);
        }
    }
}