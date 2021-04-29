using System;
using System.Threading.Tasks;

namespace TGS.Services.Wallet
{
    public interface IWalletRepository
    {
        ValueTask<Guid> ExchangeAsync(
            Guid brandId,
            Guid walletId,
            string playerCode,
            TransactionTypes transactionType,
            decimal amount);

        ValueTask<decimal> GetBalanceAsync(Guid walletId, string playerCode);
    }
}
