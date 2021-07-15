using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.PragmaticPlay;
using TGS.Shared.Game.Models.PragmaticPlay.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public sealed class PragmaticPlayGameHistory : BaseBetHistory
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IPragmaticPlayApiClient _client;
        private readonly IPragmaticPlayHashProvider _hashProvider;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public PragmaticPlayGameHistory(
            IRoundQueries roundQueries,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            ISettingOperations settingOperations,
            IPragmaticPlayHashProvider hashProvider,
            IPragmaticPlayApiClient client)
            : base(roundQueries, gameProviderOperations)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _hashProvider = hashProvider;
            _client = client;
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider,
            CachedGame cachedGame, string language)
        {
            var configuration =
                await _settingOperations
                    .GetValueFromJsonByKeyAsync<PragmaticPlayConfiguration>(SettingConsts.GameProviderConfigurations
                        .PragmaticPlay)
                    .ConfigureAwait(false);

            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode)
                .ConfigureAwait(false);

            var culture = await _brandGameProviderOperations.MapCultureAsync(cachedGameProvider.Id, brandId, language)
                .ConfigureAwait(false);

            var model = new OpenHistoryExtendedRequest
            {
                SecureLogin = configuration.SecureLogin,
                playerCode = playerCode,
                RoundId = round.GameActions.First().ExternalGameActionGroupId,
                Language = culture
            };

            var hash = _hashProvider.ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var response = await _client.GetRoundHistoryAsync(configuration.ApiUrl, request)
                .ConfigureAwait(false);

            if (response.Error.IgnoreCaseNotEquals("0"))
            {
                throw new GameProviderApiErrorException($"Get sbo bet history fail: {response.Description}");
            }

            return new()
            {
                BetAmount = round.BetAmount ?? 0,
                Winloss = (round.WinLoss ?? 0).Negate(),
                Turnover = round.Turnover ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new UrlData {Url = response.Url}
                }
            };
        }
    }
}