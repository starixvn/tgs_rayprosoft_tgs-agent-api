using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Models.Interfaces
{
    public interface IGameProviderWalletAction
    {
        ValueTask<GameProviderWalletFnudOutResponse> FnudOutAsync(GameProviderWalletFnudOutRequest request);
        ValueTask<GameProviderWalletFundInResponse> FundInAsync(GameProviderWalletFundInRequest request);
        ValueTask<GameProviderWalletGetBalanceResponse> GetBalanceAsync(GameProviderWalletGetBalanceRequest request);
    }
}
