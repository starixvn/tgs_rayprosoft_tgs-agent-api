using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Models.Interfaces
{
    public interface IGameProviderWalletAction
    {
        /// <summary>
        /// Funds the out asynchronous.
        /// Tgs -> Gp
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        ValueTask<GameProviderWalletFundResponse> FundOutAsync(GameProviderWalletFundRequest request);

        /// <summary>
        /// Funds the in asynchronous.
        /// Gp -> Tgs 
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        ValueTask<GameProviderWalletFundResponse> FundInAsync(GameProviderWalletFundRequest request);

        /// <summary>
        /// Gets the balance asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        ValueTask<GameProviderWalletGetBalanceResponse> GetBalanceAsync(GameProviderWalletGetBalanceRequest request);
    }
}
