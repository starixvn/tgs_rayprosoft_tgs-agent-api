using System;
using System.Threading.Tasks;
using Tgs.CashService.Client;

namespace TGS.Services.BackOffice.Domain
{
    internal interface IPlayerWalletExchangeService
    {
        ValueTask<TransactionResponse> CancelWithdrawalAsync(string playerCode, decimal amount, string description,
            string externalId, Guid? createdByUserId);
    }
}