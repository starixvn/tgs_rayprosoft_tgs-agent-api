using System;
using System.Threading.Tasks;
using TGS.Services.WalletService;

namespace TGS.Services.Wallet
{
    public interface IAgentWalletRepository
    {
        ValueTask<Guid> ExchangeAsync(
            Guid brandId,
            Guid walletId,
            Guid agentId,
            TransactionTypes transactionType,
            decimal amount);

        ValueTask<decimal> GetBalanceAsync(Guid walletId, string playerCode);
    }
}