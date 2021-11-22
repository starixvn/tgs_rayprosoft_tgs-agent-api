using System;
using System.Threading.Tasks;
using Tgs.CashService.Client;

namespace TGS.Services.BackOffice.Domain
{
    internal interface IPlayerWalletExchangeService
    {
        ValueTask<TransactionResponse> AdjustAsync(string playerCode, decimal amount, string description, string externalId, Guid createdByUserId);
        ValueTask<TransactionResponse> BonusAsync(string playerCode, decimal amount, string description, string externalId);
        ValueTask<TransactionResponse> CancelWithdrawalAsync(string playerCode, decimal amount, string description, string externalId, Guid? createdByUserId);
        ValueTask<TransactionResponse> DepositAsync(string playerCode, decimal amount, string description, string externalId);
        ValueTask<TransactionResponse> GameProviderDepositAsync(string playerCode, decimal amount, string description, string externalId);
        ValueTask<TransactionResponse> GameProviderFundInAsync(string playerCode, decimal amount, string description, string externalId);
        ValueTask<TransactionResponse> GameProviderFundOutAsync(string playerCode, decimal amount, string description, string externalId);
        ValueTask<TransactionResponse> GameProviderWithdrawalAsync(string playerCode, decimal amount, string description, string externalId);
        ValueTask<decimal> GetBalanceAsync(string playerCode);
        ValueTask<TransactionResponse> OtherAsync(string playerCode, decimal amount, string description, string externalId, Guid createdByUserId);
        ValueTask<TransactionResponse> PromotionAsync(string playerCode, decimal amount, string description, string externalId, Guid? createByUserId);
        ValueTask<TransactionResponse> RebateAsync(string playerCode, decimal amount, string description, string externalId);
        ValueTask<TransactionResponse> WithdrawalAsync(string playerCode, decimal amount, string description, string externalId);
    }
}