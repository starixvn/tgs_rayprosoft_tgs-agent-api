using System.Text;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Services
{
    public sealed class HabaneroGameHistory : BaseBetHistory
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IHashProvider _hashProvider;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public HabaneroGameHistory(
            IRoundQueries roundQueries,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            ILocalizationOperations localizationOperations,
            ISettingOperations settingOperations,
            IHashProvider hashProvider) :
            base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _hashProvider = hashProvider;
        }

        protected override async Task<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<HabaneroConfiguration>(SettingConsts
                    .GameProviderConfigurations.Habanero);

            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var culture = await _brandGameProviderOperations.MapCultureAsync(cachedGameProvider.Id, brandId,
                language);

            var valueToHash =
                $"{round.ExternalRoundId.ToLower()}{configuration.BrandId.ToLower()}{configuration.ApiKey.ToLower()}";

            var hash = _hashProvider.Hash(valueToHash, HashAlgorithmKind.Sha256, HashStringKind.Hex, Encoding.UTF8);

            var url = $"{configuration.BetHistoryUrl.AbsoluteUri}games/history"
                .AppendQueryString($"brandId={configuration.BrandId}")
                .AppendQueryString($"friendlyid={round.ExternalRoundId}")
                .AppendQueryString("viewtype=game")
                .AppendQueryString($"local={culture}")
                .AppendQueryString($"hash={hash}");

            return new()
            {
                BetAmount = round.BetAmount ?? 0,
                Winloss = (round.WinLoss ?? 0).Negate(),
                Turnover = round.Turnover ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[] {new UrlData {Url = url}}
            };
        }
    }
}