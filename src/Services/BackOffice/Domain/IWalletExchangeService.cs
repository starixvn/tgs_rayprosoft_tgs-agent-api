using System;
using System.Threading.Tasks;
using Tgs.Payment.Clients;

namespace TGS.Services.BackOffice.Domain
{
    internal interface IWalletExchangeService
    {
        ValueTask CancelWithdrawAsync(
            Guid brandId,
            UserType userType,
            string userId,
            decimal amount,
            string reviewUserId);
    }
}
