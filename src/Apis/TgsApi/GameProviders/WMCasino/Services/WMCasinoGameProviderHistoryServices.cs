using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.WMCasino;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;
using TGS.Shared.Game.Models.WMCasino.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoGameProviderHistoryServices
    {
        ValueTask<GameProviderGameRecordResponse> GetGameRecordAsync(GameProviderGameRecordRequest request);
    }

    public class WMCasinoGameProviderHistoryServices : IWMCasinoGameProviderHistoryServices
    {
        private readonly IWMCasinoAgentMapper _agentMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IWMCasinoApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITgsLog _log;
        private readonly IPlayerOperations _playerOperations;
        private readonly IWMCasinoPlayerServices _playerServices;
        private readonly IWMCasinoBetActionServices _services;
        private readonly ISettingOperations _settingOperations;
        private readonly IWMCasinoUtility _utility;

        public WMCasinoGameProviderHistoryServices(
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandOperations brandOperations,
            IPlayerOperations playerOperations,
            IWMCasinoApiClient client,
            IWMCasinoAgentMapper agentMapper,
            IWMCasinoBetActionServices services,
            IWMCasinoPlayerServices playerServices,
            IWMCasinoUtility utility,
            ITgsLog log)
        {
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gameProviderOperations = gameProviderOperations ??
                throw new ArgumentNullException(nameof(gameProviderOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _playerServices = playerServices ?? throw new ArgumentNullException(nameof(playerServices));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _log = log.SetLogger(nameof(WMCasinoGameProviderHistoryServices)) ??
                throw new ArgumentNullException(nameof(log));
        }

        public async ValueTask<GameProviderGameRecordResponse> GetGameRecordAsync(GameProviderGameRecordRequest request)
        {
            var configuration = await _settingOperations
                .GetValueFromJsonByKeyAsync<WMCasinoConfiguration>(SettingConsts.GameProviderConfigurations.WMCasino)
                .ConfigureAwait(false);

            var agent = _agentMapper.GetAgentById(configuration.BrandMerchants, request.AgentId);

            var response = await _client.GetDateTimeReportAsync(configuration.ApiUrl,
                new GetDateTimeReportRequest
                {
                    VendorId = agent.AgentId,
                    Signature = agent.Signature,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime
                }).ConfigureAwait(false);

            if (response.ErrorCode != (int)ServiceApiError.Success &&
                response.ErrorCode != (int)ServiceApiError.NoResult)
            {
                throw new GameProviderApiErrorException(response.ErrorMessage);
            }

            if (response.Results == null || response.Results.Length == 0)
            {
                return new GameProviderGameRecordResponse { Success = true };
            }

            var gameProviderId = await _gameProviderOperations
                .GetGameProviderIdByCodeAsync(Consts.GameProvider.WMCasino)
                .ConfigureAwait(false);

            var results = (from r in response.Results
                           let externalRoundId = _utility.GetExternalRoundIdByBetResult(r.RoundId, r.SubRoundId)
                           let playerCode = _playerServices.GetPlayerCode(r.User, agent.Prefix, agent.Suffix)
                           group r by new
                           {
                               PlayerCode = playerCode,
                               ExternalRoundId = externalRoundId
                           }
                           into g
                           select g).ToDictionary(g => g.Key, g => g.ToArray());

            var configurationIds = new ConcurrentDictionary<Guid, Guid>();

            var tasks = results.Select(async r =>
            {
                var playerCode = r.Key.PlayerCode;

                if (playerCode == "4udw8okzh5hd")
                {
                    playerCode = "4ucmqplxb029";
                }

                try
                {
                    var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);

                    if (!configurationIds.TryGetValue(brandId, out var gameProviderConfigurationId))
                    {
                        var bgpc = await _brandOperations
                            .GetBrandGameProviderConfigurationAsync(brandId, gameProviderId)
                            .ConfigureAwait(false);

                        gameProviderConfigurationId = bgpc.GameProviderConfigurationId;
                        configurationIds[brandId] = gameProviderConfigurationId;
                    }

                    await _services.ExecuteBetResultsByRoundAsync(
                            gameProviderId, gameProviderConfigurationId, playerCode, r.Value, true)
                        .ConfigureAwait(false);

                    return new
                    {
                        Success = true,
                        PlayerCode = playerCode,
                        ErrorMessage = string.Empty
                    };
                }
                catch (Exception ex)
                {
                    return new
                    {
                        Success = false,
                        PlayerCode = playerCode,
                        ErrorMessage = ex.GetMessageChain()
                    };
                }
            });

            var executeResults = await Task.WhenAll(tasks).ConfigureAwait(false);

            if (executeResults.Any(r => !r.Success))
            {
                var errors = executeResults.Where(r => !r.Success)
                    .Select(r => $"Execute round results failed, player code: {r.PlayerCode}, error: {r.ErrorMessage}");

                var message = string.Join(Environment.NewLine, errors);
                _log.LogError(message);

                return new GameProviderGameRecordResponse { Success = false };
            }

            var lastGameRecord = response.Results.Where(r => !string.IsNullOrWhiteSpace(r.SettleTime))
                .Max(r => _utility.ParseDateTimeOffset(r.SettleTime));

            return new GameProviderGameRecordResponse
            {
                Success = true,
                LastGameRecordTime = lastGameRecord
            };
        }
    }
}