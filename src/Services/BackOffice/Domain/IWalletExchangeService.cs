using System;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.Services.WalletService;

namespace TGS.Services.BackOffice.Domain
{
    internal interface IWalletExchangeService
    {
        ValueTask ExchangeAsync(
            Guid brandId,
            UserType userType,
            string userId,
            TransactionTypes transactionType,
            decimal amount);
    }
}
