// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.IMESports;
using TGS.Shared.Game.Models.IMESports.Enums;
using TGS.Shared.Game.Models.IMESports.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.IMESports.Services
{
    public sealed class IMESportsGameHistory : BaseBetHistory
    {
        private static readonly string _timeFormat = "yyyy-MM-dd HH.mm.ss";
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IIMESportsApiClient _client;
        private readonly IJsonSerializationProvider _json;
        private readonly IPlayerOperations _playerOperations;
        private readonly IIMESportsServices _services;
        private readonly ISettingOperations _settingOperations;

        public IMESportsGameHistory(
            IRoundQueries roundQueries,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IJsonSerializationProvider json,
            IIMESportsApiClient client,
            IIMESportsServices services)
            : base(roundQueries, gameProviderOperations)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _json = json;
            _client = client;
            _services = services;
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var config =
                await _settingOperations.GetValueFromJsonByKeyAsync<IMESportsConfiguration>(SettingConsts
                    .GameProviderConfigurations.IMESports);
            var merchant = await _services.GetBrandMerchantAsync(playerCode, config);

            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);
            var culture = await _brandGameProviderOperations.MapCultureAsync(cachedGameProvider.Id, brandId, language)
                .ConfigureAwait(false);

            var result = await GetHistoryResultAsync(config, merchant, round.ExternalCreatedOn!.Value, culture,
                round.ExternalRoundId);

            if (result == null)
            {
                return new();
            }

            return new()
            {
                BetAmount = result.StakeAmount,
                Turnover = result.StakeAmount,
                Winloss = result.WinLoss,
                Data = new BaseData[]
                {
                    new JsonData {Value = _json.SerializeToString(result.Details, new() {WriteIndented = false})}
                }
            };
        }

        private async Task<HistoryResult> GetHistoryResultAsync(IMESportsConfiguration config,
            IMESportsBrandMerchant merchant, DateTimeOffset externalCreatedOn, string culture, string transactionId)
        {
            var createdOn = externalCreatedOn.ToOffset(TimeSpan.FromHours(8));

            var startDate = createdOn.AddMinutes(-1).ToString(_timeFormat);
            var endDate = createdOn.AddMinutes(1).ToString(_timeFormat);
            var currentPage = 1;

            var response = await _client.GetRoundHistoryAsync(merchant.ApiUrl,
                new()
                {
                    MerchantCode = merchant.MerchantCode,
                    ProductWallet = config.ProductWallet,
                    StartDate = startDate,
                    EndDate = endDate,
                    Page = currentPage,
                    Language = culture
                }).ConfigureAwait(false);

            if (response.ErrorCode != GameApiError.Success)
            {
                throw new GameProviderApiErrorException(
                    $"Get {Consts.GameProvider.IMESports} history fail: {response.Message}");
            }

            var totalPage = response.Pagination.TotalPage;
            var result = GetHistoryResult(response, transactionId);

            if (totalPage == 1 || result != null)
            {
                return result;
            }

            currentPage++;
            while (result is null && currentPage <= totalPage)
            {
                response = await _client.GetRoundHistoryAsync(merchant.ApiUrl,
                    new()
                    {
                        MerchantCode = merchant.MerchantCode,
                        ProductWallet = config.ProductWallet,
                        StartDate = startDate,
                        EndDate = endDate,
                        Page = currentPage,
                        Language = culture
                    }).ConfigureAwait(false);
                result = GetHistoryResult(response, transactionId);
                currentPage++;
            }

            return result;
        }

        private static HistoryResult GetHistoryResult(GetRoundHistoryResponse response, string transactionId) =>
            response.Results.FirstOrDefault(r => r.BetId == transactionId);
    }
}