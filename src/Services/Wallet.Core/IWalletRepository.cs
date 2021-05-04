using System;
using System.Threading.Tasks;

namespace TGS.Services.WalletService
{
    public interface IWalletRepository
    {
        ValueTask<(Guid TranscationId, decimal PostBalance)> ExchangeAsync(
            Guid brandId,
            Guid walletId,
            string playerCode,
            TransactionTypes transactionType,
            decimal amount);

        ValueTask<decimal> GetBalanceAsync(Guid walletId, string playerCode);
    }
}
