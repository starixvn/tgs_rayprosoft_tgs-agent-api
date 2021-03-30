using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operator;

namespace TGS.Operator.Repositories
{
    internal class WalletRepository : IWalletRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public WalletRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask CreateDepositFormAsync(BankDepositForm form)
        {
            _tgsRepository.DepositForms.Add(new DepositForm
            {
                Id = Guid.NewGuid(),
                PlayerCode = form.PlayerCode,
                BrandId = form.BrandId,
                BrandPaymentId = form.BrandPaymentId,
                BankAccount = form.BankAccount,
                Amount = form.Amount
            });

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask<BalanceInfo> GetBalanceInfoAsync(string playerCode)
        {
            var playerWallet = await _tgsRepository.Wallets
                .Where(wallet => wallet.PlayerCode == playerCode)
                .SingleAsync()
                .ConfigureAwait(false);

            return new BalanceInfo(playerWallet.Balance);
        }
    }
}
