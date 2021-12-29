using System;
using System.Threading.Tasks;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.WMCasino.Common;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoBetActionServices
    {
        ValueTask<Core.Models.Services.Games.BetResult> CancelLockAmountAsync(Guid gameProviderId, Guid gameProviderConfigurationId,
            string playerCode, CancelWalletTransferRequest request);

        ValueTask ExecuteBetResultsByRoundAsync(Guid gameProviderId,
            Guid gameProviderConfigurationId, string playerCode, GameBetResult[] betResults, bool fromSchedule = false);

        ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode);

        ValueTask<Core.Models.Services.Games.BetResult> LockAmountAsync(Guid gameProviderId, Guid gameProviderConfigurationId,
            string playerCode, WalletTransferRequest request);
    }
}
