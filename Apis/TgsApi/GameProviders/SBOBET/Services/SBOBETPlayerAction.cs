// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Clients;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages.Pluggables;
using TGS.Core.Models.Services.Data;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using static TGS.Apis.TgsApi.GameModels.SBOBET.Enums.ServiceApiError;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public sealed class SBOBETPlayerAction : IPlayerAction
    {
        private readonly ISBOApiClient _client;
        private readonly ISettingOperations _settingOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IBetLimitQueries _betLimitQueries;

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

        async Task<PlayerKickResponse> IPlayerAction.KickAsync(PlayerKickRequest request)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts
                    .GameProviderConfigurations.SBOBET);

            var response = await _client.LogoutAsync(configuration.ApiUrl,
                new()
                {
                    CompanyKey = configuration.CompanyKey,
                    PlayerCode = request.PlayerCode
                });

            return response.Error.Code != None
                ? new PlayerKickResponse
                {
                    Error = (int) TgsApiError.ProviderApiError,
                    ErrorMessage = response.Error.Message
                }
                : new();
        }

        async Task<PlayerUpdateSportBetLimitsResponse> IPlayerAction.UpdateSportBetLimitsAsync(
            PlayerUpdateSportBetLimitsRequest request)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts
                    .GameProviderConfigurations.SBOBET);

            var playerCode = request.PlayerCode;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);
            var betLimits =
                await _betLimitQueries.GetPlayerSportBetLimitsAsync(playerCode, gameProviderId,
                    request.SportMarketTypeIds);

            var response = await UpdatePlayerBetLimitsAsync(playerCode, betLimits, configuration);

            return response.Error.Code != None
                ? new PlayerUpdateSportBetLimitsResponse
                {
                    Error = (int) TgsApiError.ProviderApiError,
                    ErrorMessage = response.Error.Message
                }
                : new();
        }

        private async Task<BaseResponse> UpdatePlayerBetLimitsAsync(string playerCode, SportBetLimitResult[] betLimits,
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

            var response = await _client.UpdatePlayerBetLimitsAsync(apiUrl, updateBetLimitRequest);

            if (response.Error.Code == None || response.Error.Code != UserNotExist)
            {
                return response;
            }

            var player = await _playerOperations.GetCachedPlayerAsync(playerCode);

            var registerResponse = await _client.RegisterPlayerAsync(apiUrl,
                new(player.BrandCode, player.DefaultWalletCurrencyCode)
                {
                    CompanyKey = companyKey,
                    PlayerCode = playerCode
                });

            if (registerResponse.Error.Code != None)
            {
                return registerResponse;
            }

            return await _client.UpdatePlayerBetLimitsAsync(apiUrl, updateBetLimitRequest);
        }
    }
}